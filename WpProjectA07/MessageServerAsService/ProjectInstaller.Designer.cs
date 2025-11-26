namespace MessageServerAsService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MessageServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.MessageServerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // MessageServiceProcessInstaller
            // 
            this.MessageServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.MessageServiceProcessInstaller.Password = null;
            this.MessageServiceProcessInstaller.Username = null;
            // 
            // MessageServerServiceInstaller
            // 
            this.MessageServerServiceInstaller.ServiceName = "MessageServerService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MessageServiceProcessInstaller,
            this.MessageServerServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller MessageServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller MessageServerServiceInstaller;
    }
}