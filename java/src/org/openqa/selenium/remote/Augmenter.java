// Licensed to the Software Freedom Conservancy (SFC) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The SFC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

package org.openqa.selenium.remote;

import static java.util.Collections.unmodifiableSet;
import static net.bytebuddy.matcher.ElementMatchers.anyOf;
import static net.bytebuddy.matcher.ElementMatchers.named;

import java.lang.reflect.Field;
import java.lang.reflect.Modifier;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.ServiceLoader;
import java.util.Set;
import java.util.function.BiFunction;
import java.util.function.Predicate;
import java.util.logging.Logger;
import java.util.stream.Collectors;
import java.util.stream.Stream;
import java.util.stream.StreamSupport;
import net.bytebuddy.ByteBuddy;
import net.bytebuddy.description.annotation.AnnotationDescription;
import net.bytebuddy.dynamic.DynamicType;
import net.bytebuddy.dynamic.loading.ClassLoadingStrategy;
import net.bytebuddy.implementation.FixedValue;
import net.bytebuddy.implementation.MethodDelegation;
import org.openqa.selenium.Beta;
import org.openqa.selenium.Capabilities;
import org.openqa.selenium.HasAuthentication;
import org.openqa.selenium.HasCapabilities;
import org.openqa.selenium.ImmutableCapabilities;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebDriverException;
import org.openqa.selenium.WrapsDriver;
import org.openqa.selenium.internal.Require;
import org.openqa.selenium.logging.HasLogEvents;
import org.openqa.selenium.remote.html5.AddWebStorage;
import org.openqa.selenium.support.decorators.Decorated;

/**
 * Enhance the interfaces implemented by an instance of the {@link org.openqa.selenium.WebDriver}
 * based on the returned {@link org.openqa.selenium.Capabilities} of the driver. Note: this class is
 * still experimental. Use at your own risk.
 */
@Beta
public class Augmenter {

  private static final Logger LOG = Logger.getLogger(Augmenter.class.getName());
  private final Set<Augmentation<?>> augmentations;

  public Augmenter() {
    Set<Augmentation<?>> augmentations = new HashSet<>();
    Stream.of(new AddWebStorage())
        .forEach(provider -> augmentations.add(createAugmentation(provider)));

    StreamSupport.stream(ServiceLoader.load(AugmenterProvider.class).spliterator(), false)
        .forEach(provider -> augmentations.add(createAugmentation(provider)));

    this.augmentations = unmodifiableSet(augmentations);
  }

  private static <X> Augmentation<X> createAugmentation(AugmenterProvider<X> provider) {
    Require.nonNull("Interface provider", provider);
    return new Augmentation<>(
        provider.isApplicable(), provider.getDescribedInterface(), provider::getImplementation);
  }

  private Augmenter(Set<Augmentation<?>> augmentations, Augmentation<?> toAdd) {
    Require.nonNull("Current list of augmentations", augmentations);
    Require.nonNull("Augmentation to add", toAdd);

    Set<Augmentation<?>> toUse = new HashSet<>(augmentations.size() + 1);
    toUse.addAll(augmentations);
    toUse.add(toAdd);

    this.augmentations = unmodifiableSet(toUse);
  }

  @SuppressWarnings("unused")
  public <X> Augmenter addDriverAugmentation(AugmenterProvider<X> provider) {
    Require.nonNull("Interface provider", provider);

    return addDriverAugmentation(
        provider.isApplicable(), provider.getDescribedInterface(), provider::getImplementation);
  }

  public <X> Augmenter addDriverAugmentation(
      String capabilityName,
      Class<X> implementThis,
      BiFunction<Capabilities, ExecuteMethod, X> usingThis) {
    Require.nonNull("Capability name", capabilityName);
    Require.nonNull("Interface to implement", implementThis);
    Require.nonNull("Concrete implementation", usingThis, "of %s", implementThis);

    return addDriverAugmentation(check(capabilityName), implementThis, usingThis);
  }

  public <X> Augmenter addDriverAugmentation(
      Predicate<Capabilities> whenThisMatches,
      Class<X> implementThis,
      BiFunction<Capabilities, ExecuteMethod, X> usingThis) {
    Require.nonNull("Capability predicate", whenThisMatches);
    Require.nonNull("Interface to implement", implementThis);
    Require.nonNull("Concrete implementation", usingThis, "of %s", implementThis);
    Require.precondition(
        implementThis.isInterface(), "Expected %s to be an interface", implementThis);

    return new Augmenter(
        augmentations, new Augmentation<>(whenThisMatches, implementThis, usingThis));
  }

  private Predicate<Capabilities> check(String capabilityName) {
    return caps -> {
      Require.nonNull("Capability name", capabilityName);

      Object value = caps.getCapability(capabilityName);
      if (value instanceof Boolean && !((Boolean) value)) {
        return false;
      }
      return value != null;
    };
  }

  /**
   * Enhance the interfaces implemented by this instance of WebDriver if that instance is a {@link
   * org.openqa.selenium.remote.RemoteWebDriver}. The WebDriver that is returned may well be a
   * dynamic proxy. You cannot rely on the concrete implementing class to remain constant.
   *
   * @param driver The driver to enhance
   * @return A class implementing the described interfaces.
   */
  public WebDriver augment(WebDriver driver) {
    Require.nonNull("WebDriver", driver);
    Require.precondition(
        driver instanceof HasCapabilities, "Driver must have capabilities", driver);

    if (driver instanceof Decorated<?>) {
      LOG.warning(
          "Warning: In future versions, passing a decorated driver will no longer be allowed.\n"
              + " Instead, augment the driver first and then use it to created a decorated"
              + " driver.\n"
              + " Explanation: Decorated drivers are not aware of the augmentations applied to"
              + " them. It can lead to expected behavior.\n"
              + " For example, augmenting HasDevTools interface to a decorated driver. \n"
              + " The decorated driver is not aware that after augmentation it is an instance of"
              + " HasDevTools. So it does not invoke the close() method of the underlying"
              + " websocket, potentially causing a memory leak. ");
    }

    Capabilities caps = ImmutableCapabilities.copyOf(((HasCapabilities) driver).getCapabilities());

    // Collect the interfaces to apply
    List<Augmentation<?>> matchingAugmenters =
        augmentations.stream()
            // Only consider the augmenters that match interfaces we don't already implement
            .filter(
                augmentation -> !augmentation.interfaceClass.isAssignableFrom(driver.getClass()))
            // And which match an augmentation we have
            .filter(augmentation -> augmentation.whenMatches.test(caps))
            .collect(Collectors.toList());

    return this.augment(driver, matchingAugmenters);
  }

  private WebDriver augment(WebDriver driver, List<Augmentation<?>> matchingAugmenters) {
    Capabilities caps = ImmutableCapabilities.copyOf(((HasCapabilities) driver).getCapabilities());

    if (matchingAugmenters.isEmpty()) {
      return driver;
    }

    // Grab a remote execution method, if possible
    RemoteWebDriver remote = extractRemoteWebDriver(driver);
    ExecuteMethod execute =
        remote == null
            ? (commandName, parameters) -> {
              throw new WebDriverException("Cannot execute remote command: " + commandName);
            }
            : new RemoteExecuteMethod(remote);

    DynamicType.Builder<? extends WebDriver> builder =
        new ByteBuddy()
            .subclass(driver.getClass())
            .annotateType(AnnotationDescription.Builder.ofType(Augmentable.class).build())
            .method(named("isAugmented"))
            .intercept(FixedValue.value(true));

    for (Augmentation<?> augmentation : matchingAugmenters) {
      Class<?> iface = augmentation.interfaceClass;

      Object instance = augmentation.implementation.apply(caps, execute);

      builder =
          builder
              .implement(iface)
              .method(anyOf(iface.getDeclaredMethods()))
              .intercept(MethodDelegation.to(instance, iface));
    }

    Class<? extends WebDriver> definition =
        builder
            .make()
            .load(driver.getClass().getClassLoader(), ClassLoadingStrategy.Default.WRAPPER)
            .getLoaded()
            .asSubclass(driver.getClass());

    try {
      WebDriver toReturn = definition.getDeclaredConstructor().newInstance();

      copyFields(driver.getClass(), driver, toReturn);

      toReturn = addDependentAugmentations(toReturn);

      return toReturn;
    } catch (ReflectiveOperationException e) {
      throw new IllegalStateException("Unable to create new proxy", e);
    }
  }

  private WebDriver addDependentAugmentations(WebDriver driver) {
    List<Augmentation<?>> augmentationList = new ArrayList<>();

    WebDriver toReturn = driver;

    // add interfaces that need to use the augmented driver
    if (!(driver instanceof HasAuthentication)) {
      augmentationList.add(createAugmentation(new AddHasAuthentication()));
    }

    if (!(driver instanceof HasLogEvents)) {
      augmentationList.add(createAugmentation(new AddHasLogEvents()));
    }

    if (!augmentationList.isEmpty()) {
      Capabilities caps =
          ImmutableCapabilities.copyOf(((HasCapabilities) driver).getCapabilities());

      List<Augmentation<?>> matchingAugmenters =
          augmentationList.stream()
              .filter(augmentation -> augmentation.whenMatches.test(caps))
              .collect(Collectors.toList());

      toReturn = this.augment(driver, matchingAugmenters);
    }
    return toReturn;
  }

  private RemoteWebDriver extractRemoteWebDriver(WebDriver driver) {
    Require.nonNull("WebDriver", driver);

    if (driver instanceof RemoteWebDriver) {
      return (RemoteWebDriver) driver;
    }

    if (driver instanceof Decorated) {
      return extractRemoteWebDriver((WebDriver) ((Decorated<?>) driver).getOriginal());
    }

    if (driver instanceof WrapsDriver) {
      return extractRemoteWebDriver(((WrapsDriver) driver).getWrappedDriver());
    }

    return null;
  }

  private void copyFields(Class<?> clazz, Object source, Object target) {
    if (Object.class.equals(clazz)) {
      // Stop!
      return;
    }

    for (Field field : clazz.getDeclaredFields()) {
      copyField(source, target, field);
    }

    copyFields(clazz.getSuperclass(), source, target);
  }

  private void copyField(Object source, Object target, Field field) {
    if (Modifier.isFinal(field.getModifiers())) {
      return;
    }

    try {
      field.setAccessible(true);
      Object value = field.get(source);
      field.set(target, value);
    } catch (IllegalAccessException e) {
      throw new RuntimeException(e);
    }
  }

  private static class Augmentation<X> {
    public final Predicate<Capabilities> whenMatches;
    public final Class<X> interfaceClass;
    public final BiFunction<Capabilities, ExecuteMethod, X> implementation;

    public Augmentation(
        Predicate<Capabilities> whenMatches,
        Class<X> interfaceClass,
        BiFunction<Capabilities, ExecuteMethod, X> implementation) {
      this.whenMatches = Require.nonNull("Capabilities predicate", whenMatches);
      this.interfaceClass = Require.nonNull("Interface to implement", interfaceClass);
      this.implementation = Require.nonNull("Interface implementation", implementation);

      Require.precondition(
          interfaceClass.isInterface(),
          "%s must be an interface, not a concrete class",
          interfaceClass);
    }
  }
}
