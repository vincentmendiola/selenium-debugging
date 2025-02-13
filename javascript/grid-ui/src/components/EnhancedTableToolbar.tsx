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

import React from 'react'
import Toolbar from '@mui/material/Toolbar'
import Typography from '@mui/material/Typography'
import Box from '@mui/material/Box'

interface EnhancedTableToolbarProps {
  title: string
  children?: JSX.Element
}

function EnhancedTableToolbar (props: EnhancedTableToolbarProps) {
  const {
    title,
    children
  } = props

  return (
    <Toolbar sx={{ paddingLeft: 2, paddingRight: 1 }}>
      <Typography
        textAlign='center'
        sx={{ flex: '1 1 100%' }}
        variant='h3'
        id='tableTitle'
        component='div'
      >
        <Box
          component='span'
          display='flex'
          alignItems='center'
        >
          <Box
            component='span'
            display='flex'
            justifyContent='center'
            flex={1}
          >
            {title}
          </Box>
          {children}
        </Box>
      </Typography>
    </Toolbar>
  )
}

export default EnhancedTableToolbar
