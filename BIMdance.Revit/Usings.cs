﻿global using System;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Data;
global using System.Diagnostics;
global using System.Drawing;
global using System.Drawing.Imaging;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Resources;
global using System.Runtime.CompilerServices;
global using System.Runtime.ExceptionServices;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows;
global using System.Windows.Data;
global using System.Windows.Interop;
global using System.Windows.Media;
global using System.Windows.Media.Imaging;

global using Autodesk.Revit.ApplicationServices;
global using Autodesk.Revit.DB;
global using Autodesk.Revit.DB.Architecture;
global using Autodesk.Revit.DB.Electrical;
global using Autodesk.Revit.UI;
global using Autodesk.Revit.UI.Events;
global using Autodesk.Revit.UI.Selection;
global using Autodesk.Windows;

global using Kivikko.Json;

global using BIMdance.Revit.Application;
global using BIMdance.Revit.Logic.CableRouting;
global using BIMdance.Revit.Logic.CableRouting.Model;
global using BIMdance.Revit.Logic.CableRouting.Utils;
global using BIMdance.Revit.Resources;
global using BIMdance.Revit.Utils;
global using BIMdance.Revit.Utils.Common;
global using BIMdance.Revit.Utils.DependencyInjection;
global using BIMdance.Revit.Utils.Observer;
global using BIMdance.Revit.Utils.Revit;
global using BIMdance.Revit.Utils.Revit.Async;
global using BIMdance.Revit.Utils.Revit.Parameters;
global using BIMdance.Revit.Utils.Revit.Parameters.Shared;
global using BIMdance.Revit.Utils.Revit.RevitProxy;
global using BIMdance.Revit.Utils.Revit.RevitVersions;
global using BIMdance.Revit.Utils.Revit.Ribbon;
global using BIMdance.Revit.Utils.Revit.Ribbon.Bindings;
global using BIMdance.Revit.Utils.Revit.Ribbon.Definitions;
global using BIMdance.Revit.Utils.Revit.Updaters;
