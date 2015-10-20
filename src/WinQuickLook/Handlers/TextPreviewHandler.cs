﻿using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using WinQuickLook.Interop;

namespace WinQuickLook.Handlers
{
    public class TextPreviewHandler : IQuickLookHandler
    {
        public bool CanOpen(string fileName)
        {
            var extension = (Path.GetExtension(fileName) ?? "").ToLower();

            return ((IList)_supportFormats).Contains(extension);
        }

        public FrameworkElement GetElement(string fileName)
        {
            var maxWidth = SystemParameters.WorkArea.Width - 100;
            var maxHeight = SystemParameters.WorkArea.Height - 100;

            var contents = File.ReadAllBytes(fileName);
            var encoding = DetectEncoding(contents);

            var textBox = new TextBox();

            textBox.BeginInit();
            textBox.Width = maxWidth / 2;
            textBox.Height = maxHeight / 2;
            textBox.Text = encoding.GetString(contents);
            textBox.IsReadOnly = true;
            textBox.IsReadOnlyCaretVisible = false;
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.EndInit();

            return textBox;
        }

        public bool AllowsTransparency => true;

        private static readonly string[] _supportFormats =
        {
            ".txt", ".log", ".md", ".markdown", ".xml"
        };

        private static Encoding DetectEncoding(byte[] contents)
        {
            if (contents.Length == 0)
            {
                return Encoding.ASCII;
            }

            var multiLanguage2 = (IMultiLanguage2)Activator.CreateInstance(CLSID.MultiLanguageType);

            int scores = 1;
            int length = contents.Length;

            DetectEncodingInfo encodingInfo;

            var handle = GCHandle.Alloc(contents, GCHandleType.Pinned);
            var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(contents, 0);

            try
            {
                multiLanguage2.DetectInputCodepage(0, 0, ptr, length, out encodingInfo, ref scores);
            }
            finally
            {
                handle.Free();
            }

            Marshal.FinalReleaseComObject(multiLanguage2);

            return Encoding.GetEncoding(encodingInfo.nCodePage);
        }
    }
}
