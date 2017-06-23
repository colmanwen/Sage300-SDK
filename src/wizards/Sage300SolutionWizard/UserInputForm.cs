﻿// The MIT License (MIT) 
// Copyright (c) 1994-2017 The Sage Group plc or its licensors.  All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of 
// this software and associated documentation files (the "Software"), to deal in 
// the Software without restriction, including without limitation the rights to use, 
// copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Sage.CA.SBS.ERP.Sage300.SolutionWizard.Properties;

namespace Sage.CA.SBS.ERP.Sage300.SolutionWizard
{
    public partial class UserInputForm : Form
    {
        #region Private Vars
        /// <summary> Wizard Steps </summary>
        private readonly List<WizardStep> _wizardSteps = new List<WizardStep>();

        /// <summary> Current Wizard Step </summary>
        private int _currentWizardStep;

        /// <summary> Generate </summary>
        private bool _generate = false;
        #endregion

        #region Private Consts
        /// <summary> Splitter Distance </summary>
        private const int SplitterDistance = 237;

        #endregion

        #region Public vars
        public string ThirdPartyCompanyName { get; set; }
        public string ThirdPartyApplicationId { get; set; }
        public string CompanyNamespace { get; set; }
        public string KendoFolder { get; set; }
        public string KendoDefaultFolder { set { txtKendoFolder.Text = value; }}
        public bool IncludeEnglish { get; set; }
        public bool IncludeChineseSimplified { get; set; }
        public bool IncludeChineseTraditional { get; set; }
        public bool IncludeSpanish { get; set; }
        public bool IncludeFrench { get; set; }
        #endregion

        #region Constructor
        public UserInputForm()
        {
            InitializeComponent();
            Localize();
            InitWizardSteps();
        }
        #endregion

        /// <summary> Localize </summary>
        private void Localize()
        {
            Text = Resources.SolutionGeneration;

            btnBack.Text = Resources.Back;
            btnNext.Text = Resources.Next;

            // Main Step
            lblCompanyName.Text = Resources.CompanyName;
            tooltip.SetToolTip(lblCompanyName, Resources.CompanyNameTip);

            lblModuleId.Text = Resources.ModuleId;
            tooltip.SetToolTip(lblModuleId, Resources.ModuleIdTip);

            lblNamespace.Text = Resources.NamespaceName;
            tooltip.SetToolTip(lblNamespace, Resources.NamespaceNameTip);

            // Kendo Step
            chkKendoLicense.Text = Resources.KendoLicense;
            tooltip.SetToolTip(chkKendoLicense, Resources.KendoLicenseTip);

            lblKendoFolder.Text = Resources.KendoFolder;
            tooltip.SetToolTip(lblKendoFolder, Resources.KendoFolderTip);

            tooltip.SetToolTip(btnKendoDialog, Resources.KendoFolderDialog);

            lblKendoFolderHelp.Text = Resources.KendoFolderLinkTip;

            // Resource Step
            chkEnglish.Text = Resources.English;
            tooltip.SetToolTip(chkEnglish, Resources.EnglishTip);

            chkSpanish.Text = Resources.Spanish;
            tooltip.SetToolTip(chkSpanish, string.Format(Resources.NonEnglishTip, Resources.Spanish));

            chkFrench.Text = Resources.French;
            tooltip.SetToolTip(chkFrench, string.Format(Resources.NonEnglishTip, Resources.French));

            chkChineseSimplified.Text = Resources.ChineseSimplified;
            tooltip.SetToolTip(chkChineseSimplified, string.Format(Resources.NonEnglishTip, Resources.ChineseSimplified));

            chkChineseTraditional.Text = Resources.ChineseTraditional;
            tooltip.SetToolTip(chkChineseTraditional, string.Format(Resources.NonEnglishTip, Resources.ChineseTraditional));

            // Generate Step
            lblGenerateHelp.Text = Resources.GenerateTip;

        }

        /// <summary> Initialize wizard steps </summary>
        private void InitWizardSteps()
        {
            // Default
            btnBack.Enabled = false;

            // Current Step
            _currentWizardStep = -1;

            // Init wizard steps
            _wizardSteps.Clear();

            // Init Panels
            InitPanel(pnlInfo);
            InitPanel(pnlKendo);
            InitPanel(pnlResourceFiles);
            InitPanel(pnlGenerateSolution);

            AddStep(Resources.StepTitleInfo, Resources.StepDescriptionInfo, pnlInfo);
            AddStep(Resources.StepTitleKendo, Resources.StepDescriptionKendo, pnlKendo);
            AddStep(Resources.StepTitleResourceFiles, Resources.StepDescriptionResourceFiles, pnlResourceFiles);
            AddStep(Resources.StepTitleGenerate, Resources.StepDescriptionGenerate, pnlGenerateSolution);

            // Display first step
            NextStep();
        }

        /// <summary> Initialize panel </summary>
        private static void InitPanel(Control panel)
        {
            panel.Visible = false;
            panel.Dock = DockStyle.None;
        }

        /// <summary> Add wizard steps </summary>
        /// <param name="title">Title for wizard step</param>
        /// <param name="description">Description for wizard step</param>
        /// <param name="panel">Panel for wizard step</param>
        private void AddStep(string title, string description, Panel panel)
        {
            _wizardSteps.Add(new WizardStep
            {
                Title = title,
                Description = description,
                Panel = panel
            });
        }

        /// <summary> Next/Generate Navigation </summary>
        /// <remarks>Next wizard step or Generate if last step</remarks>
        private void NextStep()
        {
            // Proceed to next wizard step or start code generation if last step
            if (!_currentWizardStep.Equals(-1) && _wizardSteps[_currentWizardStep].Panel.Name.Equals("pnlGenerateSolution"))
            {
                // Validations
                if (!ValidSettings())
                {
                    return;
                }

                // Valid!
                _generate = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                // Proceed to next step
                if (!_currentWizardStep.Equals(-1))
                {
                    btnBack.Enabled = true;

                    ShowStep(false);
                }

                _currentWizardStep++;

                ShowStep(true);

                // Update text of Next button?
                if (_wizardSteps[_currentWizardStep].Panel.Name.Equals("pnlGenerateSolution"))
                {
                    btnNext.Text = Resources.Generate;
                }

                // Update title and text for step
                ShowStepTitle();
            }
        }

        /// <summary> Back Navigation </summary>
        /// <remarks>Back wizard step</remarks>
        private void BackStep()
        {
            if (!_currentWizardStep.Equals(0))
            {
                // Proceed back a step
                if (_wizardSteps[_currentWizardStep].Panel.Name.Equals("pnlGenerateSolution"))
                {
                    btnNext.Text = Resources.Next;
                }

                ShowStep(false);

                _currentWizardStep--;

                ShowStep(true);

                // Enable back button?
                if (_currentWizardStep.Equals(0))
                {
                    btnBack.Enabled = false;
                    btnNext.Enabled = true;
                }

                // Update title and text for step
                ShowStepTitle();
            }
        }

        /// <summary> Show Step </summary>
        /// <param name="visible">True to show otherwise false </param>
        private void ShowStep(bool visible)
        {
            _wizardSteps[_currentWizardStep].Panel.Dock = visible ? DockStyle.Fill : DockStyle.None;
            _wizardSteps[_currentWizardStep].Panel.Visible = visible;
            splitSteps.SplitterDistance = SplitterDistance;
        }

        /// <summary> Show Step Title</summary>
        private void ShowStepTitle()
        {
            lblStepTitle.Text = Resources.Step + (_currentWizardStep + 1).ToString("#0") + Resources.Dash +
                                _wizardSteps[_currentWizardStep].Title;
            lblStepDescription.Text = _wizardSteps[_currentWizardStep].Description;
        }
        /// <summary> Valid Settings </summary>
        /// <returns>True if settings are valid otherwise false</returns>
        private bool ValidSettings()
        {
            // Company Name Validation
            ThirdPartyCompanyName = txtCompanyName.Text.Trim();
            if (string.IsNullOrEmpty(ThirdPartyCompanyName))
            {
                DisplayMessage(Resources.CompanyNameInvalid, MessageBoxIcon.Error);
                return false;
            }

            // Module ID Validation
            ThirdPartyApplicationId = txtApplicationID.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(ThirdPartyApplicationId) || ThirdPartyApplicationId.Contains(" "))
            {
                DisplayMessage(Resources.ModuleIdInvalid, MessageBoxIcon.Error);
                return false;
            }

            // Namespace Validation
            CompanyNamespace = txtNamespace.Text.Trim();
            if (string.IsNullOrEmpty(CompanyNamespace) || CompanyNamespace.Contains(" "))
            {
                DisplayMessage(Resources.NamespaceInvalid, MessageBoxIcon.Error);
                return false;
            }

            // Kendo License Validation
            KendoFolder = txtKendoFolder.Text.Trim();
            if (!chkKendoLicense.Checked)
            {
                DisplayMessage(Resources.KendoLicenseInvalid, MessageBoxIcon.Error);
                return false;
            }

            // Kendo Folder Validation
            if (string.IsNullOrEmpty(KendoFolder))
            {
                DisplayMessage(Resources.KendoFolderInvalid, MessageBoxIcon.Error);
                return false;
            }


            // Resources
            IncludeEnglish = chkEnglish.Checked;
            IncludeChineseSimplified = chkChineseSimplified.Checked;
            IncludeChineseTraditional = chkChineseTraditional.Checked;
            IncludeSpanish = chkSpanish.Checked;
            IncludeFrench = chkFrench.Checked;

            // Valid
            return true;
        }

        /// <summary> Generic routine for displaying a message dialog </summary>
        /// <param name="message">Message to display</param>
        /// <param name="icon">Icon to display</param>
        /// <param name="args">Message arguments, if any</param>
        private void DisplayMessage(string message, MessageBoxIcon icon, params object[] args)
        {
            MessageBox.Show(string.Format(message, args), Text, MessageBoxButtons.OK, icon);
        }

        /// <summary> Defaults Namespace </summary>
        private void SetNamespace()
        {
            ThirdPartyCompanyName = txtCompanyName.Text.Trim();

            txtNamespace.Text = !string.IsNullOrEmpty(ThirdPartyCompanyName) ? ThirdPartyCompanyName.Replace(" ", "") : "";
        }

        /// <summary> Company Name Text Changed </summary>
        /// <param name="sender">Sender object </param>
        /// <param name="e">Event Args </param>
        /// <remarks>Defaults namespace</remarks>
        private void txtCompanyName_TextChanged(object sender, EventArgs e)
        {
            SetNamespace();
        }

        /// <summary> Next/Generate toolbar button </summary>
        /// <param name="sender">Sender object </param>
        /// <param name="e">Event Args </param>
        /// <remarks>Next wizard step or Generate if last step</remarks>
        private void btnNext_Click(object sender, EventArgs e)
        {
            NextStep();
        }

        /// <summary> Back toolbar button </summary>
        /// <param name="sender">Sender object </param>
        /// <param name="e">Event Args </param>
        /// <remarks>Back wizard step</remarks>
        private void btnBack_Click(object sender, EventArgs e)
        {
            BackStep();
        }

        /// <summary> Kendo Folder search dialog</summary>
        /// <param name="sender">Sender object </param>
        /// <param name="e">Event Args </param>
        private void btnKendoDialog_Click(object sender, EventArgs e)
        {
            // Init dialog
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = false
            };

            // Show the dialog and evaluate action
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            txtKendoFolder.Text = dialog.SelectedPath.Trim();
        }

        /// <summary> Help Button</summary>
        /// <param name="sender">Sender object </param>
        /// <param name="e">Event Args </param>
        /// <remarks>Disabled help until DPP wiki is available</remarks>
        private void UserInputForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            // Display wiki link
            // System.Diagnostics.Process.Start(Resources.Browser, Resources.WikiLink);
        }

        /// <summary> Close Button</summary>
        /// <param name="sender">Sender object </param>
        /// <param name="e">Event Args </param>
        private void UserInputForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_generate)
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary> License Checkbox</summary>
        /// <param name="sender">Sender object </param>
        /// <param name="e">Event Args </param>
        private void chkKendoLicense_CheckedChanged(object sender, EventArgs e)
        {
            lblKendoFolder.Enabled = chkKendoLicense.Checked;
            txtKendoFolder.Enabled = chkKendoLicense.Checked;
            btnKendoDialog.Enabled = chkKendoLicense.Checked;
        }

        /// <summary> Kendo License Link</summary>
        /// <param name="sender">Sender object </param>
        /// <param name="e">Event Args </param>
        private void lblKendoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited.
            lblKendoLink.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start(lblKendoLink.Text);
        }

    }
}
