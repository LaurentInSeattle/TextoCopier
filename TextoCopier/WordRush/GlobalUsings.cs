
#region System + MSFT 

global using System;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows.Input;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

#endregion System 

#region Avalonia 

global using Avalonia;
global using Avalonia.Controls;
global using Avalonia.Controls.ApplicationLifetimes;
global using Avalonia.Controls.Shapes;
global using Avalonia.Data;
global using Avalonia.Data.Converters;
global using Avalonia.Data.Core.Plugins;
global using Avalonia.Input;
global using Avalonia.Input.Platform;
global using Avalonia.Interactivity;
global using Avalonia.Markup.Xaml;
global using Avalonia.Markup.Xaml.Styling;
global using Avalonia.Media;
global using Avalonia.Media.Imaging;
global using Avalonia.Media.Immutable;
global using Avalonia.Platform;
global using Avalonia.Threading;

#endregion Avalonia 

#region Framework 

global using Lyt.Avalonia.Interfaces;
global using Lyt.Avalonia.Interfaces.Logger;
global using Lyt.Avalonia.Interfaces.Messenger;
global using Lyt.Avalonia.Interfaces.Model;
global using Lyt.Avalonia.Interfaces.Profiler;
global using Lyt.Avalonia.Interfaces.Random;
global using Lyt.Avalonia.Interfaces.UserInterface;

global using Lyt.Avalonia.Controls;
global using Lyt.Avalonia.Controls.Glyphs;
global using Lyt.Avalonia.Controls.Logging;
global using Lyt.Avalonia.Controls.Toasting;

global using Lyt.Avalonia.Mvvm;
global using Lyt.Avalonia.Mvvm.Animations;
global using Lyt.Avalonia.Mvvm.Core;
global using Lyt.Avalonia.Mvvm.Dialogs;
global using Lyt.Avalonia.Mvvm.Extensions;
global using Lyt.Avalonia.Mvvm.Interfaces.Animations;
global using Lyt.Avalonia.Mvvm.Messenger;
global using Lyt.Avalonia.Mvvm.Utilities;

global using Lyt.Avalonia.Localizer;
global using Lyt.Avalonia.Model;
global using Lyt.Avalonia.Persistence;

#endregion Framework 

global using Lyt.WordRush;
global using Lyt.WordRush.Messaging;
global using Lyt.WordRush.Model;
global using Lyt.WordRush.Model.History;
global using Lyt.WordRush.Model.Parsers;
global using Lyt.WordRush.Shell;
global using Lyt.WordRush.Utilities;
global using Lyt.WordRush.Workflow.Countdown;
global using Lyt.WordRush.Workflow.Game;
global using Lyt.WordRush.Workflow.Results;
global using Lyt.WordRush.Workflow.Setup;
