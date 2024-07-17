﻿#pragma checksum "..\..\..\..\Views\DDHAuthorizationWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "35C45EC8B5DD8639BAD77795A57833C1CD73896060266077BAE007C92C6F43A6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DynamicDataHub.Views;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace DynamicDataHub.Views {
    
    
    /// <summary>
    /// DDHAuthorization
    /// </summary>
    public partial class DDHAuthorization : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox DBMSComboBox;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock NameDBServerBlock;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NameDBServerBox;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OpenExplorer;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock NameDBBlock;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NameDBBox;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ConnectionButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DynamicDataHub;component/views/ddhauthorizationwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
            ((DynamicDataHub.Views.DDHAuthorization)(target)).LocationChanged += new System.EventHandler(this.Window_LocationChanged);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
            ((DynamicDataHub.Views.DDHAuthorization)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.DBMSComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 29 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
            this.DBMSComboBox.DropDownOpened += new System.EventHandler(this.DBMSComboBox_DropDownOpened);
            
            #line default
            #line hidden
            
            #line 29 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
            this.DBMSComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DBMSComboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.NameDBServerBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.NameDBServerBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.OpenExplorer = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
            this.OpenExplorer.Click += new System.Windows.RoutedEventHandler(this.OpenExplorer_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.NameDBBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.NameDBBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.ConnectionButton = ((System.Windows.Controls.Button)(target));
            
            #line 46 "..\..\..\..\Views\DDHAuthorizationWindow.xaml"
            this.ConnectionButton.Click += new System.Windows.RoutedEventHandler(this.ConnectionButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

