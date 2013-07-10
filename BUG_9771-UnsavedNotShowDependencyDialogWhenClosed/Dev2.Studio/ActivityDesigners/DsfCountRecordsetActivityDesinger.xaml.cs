﻿using System;
using System.Activities.Presentation.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Unlimited.Applications.BusinessDesignStudio.Activities
{
    public partial class DsfCountRecordsetActivityDesigner : IDisposable
    {
        private bool _isRegistered = false;
        private string mediatorKey = string.Empty;
        public DsfCountRecordsetActivityDesigner()
        {
            InitializeComponent();
        }


        protected override void OnModelItemChanged(object newItem)
        {
            base.OnModelItemChanged(newItem);
            if (!_isRegistered)
            {
                //mediatorKey = Mediator.RegisterToReceiveMessage(MediatorMessages.DataListItemSelected, input => Highlight(input as IDataListItemModel));
            }

            ModelItem item = newItem as ModelItem;


            ModelItem parent = item.Parent;

            while (parent != null)
            {
                if (parent.Properties["Argument"] != null)
                {
                    break;
                }

                parent = parent.Parent;
            }
        }

        //private void Highlight(IDataListItemModel dataListItemViewModel) {
        //    List<string> containingFields = new List<string>();            
        //    border.Visibility = Visibility.Hidden;

        //    ForEverytxt.BorderBrush = Brushes.LightGray;
        //    ForEverytxt.BorderThickness = new Thickness(1.0);
        //    Tranformtxt.BorderBrush = Brushes.LightGray;
        //    Tranformtxt.BorderThickness = new Thickness(1.0);

        //    containingFields = DsfActivityDataListComparer.ContainsDataListItem(ModelItem, dataListItemViewModel);

        //    if (containingFields.Count > 0) {
        //        foreach (string item in containingFields) {
        //            if (item.Equals("foreachElementName")) {
        //                ForEverytxt.BorderBrush = System.Windows.Media.Brushes.Aqua;
        //                ForEverytxt.BorderThickness = new Thickness(2.0);
        //            }
        //            else if (item.Equals("additionalData")) {
        //                Tranformtxt.BorderBrush = System.Windows.Media.Brushes.Aqua;
        //                Tranformtxt.BorderThickness = new Thickness(2.0);
        //            }
        //        }

        //    }
        //}

        public void Dispose()
        {
            //Mediator.DeRegister(MediatorMessages.DataListItemSelected, mediatorKey);
        }

        void Recordsettxt_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                if (textBox.Text.EndsWith("]]"))
                {
                    if (!textBox.Text.Contains("()"))
                    {
                        textBox.Text = textBox.Text.Insert(textBox.Text.IndexOf("]"), "()");
                    }
                }
                else
                {
                    textBox.Text += "()";
                }
            }
        }

        //DONT TAKE OUT... This has been done so that the drill down doesnt happen.
        void DsfCountRecordsetActivityDesigner_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void DsfCountRecordsetActivityDesigner_OnMouseEnter(object sender, MouseEventArgs e)
        {
            UIElement uiElement = VisualTreeHelper.GetParent(this) as UIElement;
            if (uiElement != null)
            {
                Panel.SetZIndex(uiElement, int.MaxValue);
            }
        }

        void DsfCountRecordsetActivityDesigner_OnMouseLeave(object sender, MouseEventArgs e)
        {
            UIElement uiElement = VisualTreeHelper.GetParent(this) as UIElement;
            if (uiElement != null)
            {
                Panel.SetZIndex(uiElement, int.MinValue);
            }
        }
    }
}
