﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using OpenTK;
using System.Diagnostics;

namespace Examples
{
    public partial class ExampleLauncher : Form
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Form exampleLauncher = new ExampleLauncher())
            {
                Application.EnableVisualStyles();
                Application.Run(exampleLauncher);
            }
        }

        public ExampleLauncher()
        {
            InitializeComponent();

            System.Diagnostics.Debug.Listeners.Clear();
            System.Diagnostics.Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            System.Diagnostics.Debug.AutoFlush = true;
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            System.Diagnostics.Trace.AutoFlush = true;
            Trace.AutoFlush = true;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                Type example =
                    Assembly.GetExecutingAssembly().GetType(
                        "Examples." + listBox1.SelectedItem.ToString().Replace(": ", ".").Replace(' ', '_'),
                        true,
                        true
                    );

                if (example.BaseType == typeof(GameWindow))
                {
                    // Start the GameWindow in a new thread - it runs its own message loop, and it would
                    // interfere with the message loop of the ExampleLauncher.
                    new Thread(Launch).Start(example);
                }
                else if (example.BaseType == typeof(Form))
                {
                    try
                    {
                        // In this we do not want a different thread: these examples rely on the Application.Idle
                        // event, which would then be raised by both the ExampleLauncher thread *and* the new one!
                        this.AddOwnedForm((Form)example.GetConstructor(Type.EmptyTypes).Invoke(null));
                    }
                    catch (Exception expt)
                    {
                        MessageBox.Show(
                            String.Format(
                                "Stacktrace:{0}{1}{0}{0}Inner exception:{0}{2}",
                                System.Environment.NewLine,
                                expt.StackTrace,
                                expt.InnerException
                            ),
                            expt.Message
                        );
                    }
                }
            }
        }

        void Launch(object example)
        {
            try
            {
                (example as Type).InvokeMember("Launch", BindingFlags.InvokeMethod, null, null, null);
            }
            catch (Exception expt)
            {
                System.Diagnostics.Debug.WriteLine(
                    String.Format(
                        "Exception: {3}{0}Stacktrace:{0}{1}{0}{0}Inner exception:{0}{2}",
                        System.Environment.NewLine,
                        expt.StackTrace,
                        expt.InnerException,
                        expt.Message
                    )
                );
                MessageBox.Show(
                    String.Format(
                        "Stacktrace:{0}{1}{0}{0}Inner exception:{0}{2}",
                        System.Environment.NewLine,
                        expt.StackTrace,
                        expt.InnerException
                    ),
                    expt.Message
                );
            }
        }

        public void ExampleLauncher_Load(object sender, EventArgs e)
        {
            SortedList<string, string> sl = new SortedList<string, string>();

            // Get all examples
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                if (type.GetInterface("IExample") != null)
                {
                    sl.Add(
                        (type.Namespace.Replace("Examples.", String.Empty) + ": " + type.Name).Replace('_', ' '),
                        null
                    );
                }
            }

            foreach (string s in sl.Keys)
                listBox1.Items.Add(s);

            // Select first item
            if (listBox1.Items.Count > 0)
            {
                this.listBox1.SelectedIndex = 0;
            }
        }
    }
}