﻿#pragma checksum "..\..\..\DateChooserWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0D475CDEED3078B5162FA44342963B4B2E4BBD93"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PenrynAc {
    
    
    /// <summary>
    /// DateChooserWindow
    /// </summary>
    public partial class DateChooserWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\DateChooserWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextblockToday;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\DateChooserWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ListboxPicker;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\DateChooserWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ListboxDates;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\DateChooserWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextblockSelectedDate;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\DateChooserWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonSelect;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\DateChooserWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonReset;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\DateChooserWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonCancel;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\DateChooserWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CheckboxFuture;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.9.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PenrynAc;component/datechooserwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\DateChooserWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.9.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\DateChooserWindow.xaml"
            ((PenrynAc.DateChooserWindow)(target)).ContentRendered += new System.EventHandler(this.Window_ContentRendered);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TextblockToday = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.ListboxPicker = ((System.Windows.Controls.ListBox)(target));
            
            #line 20 "..\..\..\DateChooserWindow.xaml"
            this.ListboxPicker.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ListboxPicker_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ListboxDates = ((System.Windows.Controls.ListBox)(target));
            
            #line 30 "..\..\..\DateChooserWindow.xaml"
            this.ListboxDates.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ListboxDates_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.TextblockSelectedDate = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.ButtonSelect = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\..\DateChooserWindow.xaml"
            this.ButtonSelect.Click += new System.Windows.RoutedEventHandler(this.ButtonSelect_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ButtonReset = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\..\DateChooserWindow.xaml"
            this.ButtonReset.Click += new System.Windows.RoutedEventHandler(this.ButtonReset_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.ButtonCancel = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\..\DateChooserWindow.xaml"
            this.ButtonCancel.Click += new System.Windows.RoutedEventHandler(this.ButtonCancel_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.CheckboxFuture = ((System.Windows.Controls.CheckBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

