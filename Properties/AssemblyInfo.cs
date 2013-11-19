/*
Copyright (C) 2013 Jay Satiro <raysatiro@yahoo.com>
All rights reserved.

This file is part of the AlertMe extension for Fiddler.
https://github.com/jay/Fiddler-AlertMe

AlertMe is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

AlertMe is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with AlertMe.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("AlertMe")]
[assembly: AssemblyDescription("Alert when a user specified regex matches data in Fiddler.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Jay Satiro")]
[assembly: AssemblyProduct("AlertMe - https://github.com/jay/Fiddler-AlertMe")]
[assembly: AssemblyCopyright("Copyright (C) 2013 Jay Satiro <raysatiro@yahoo.com>")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]


[assembly: AssemblyVersion("1.0.1.0")]
[assembly: Fiddler.RequiredVersion("4.4.5.6")]
