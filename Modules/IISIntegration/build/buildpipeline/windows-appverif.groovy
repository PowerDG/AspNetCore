@Library('dotnet-ci') _

// 'node' indicates to Jenkins that the enclosed block runs on a node that matches
// the label 'windows-with-vs'
simpleNode('Windows.10.Amd64.EnterpriseRS3.ASPNET.Open') {
    stage ('Checking out source') {
        checkout scm
        bat 'git submodule update --init --recursive'
    }
    stage ('Build') {
        def logFolder = getLogFolder()
        def environment = "\$env:ASPNETCORE_TEST_LOG_DIR='${WORKSPACE}\\${logFolder}'"
        bat "powershell -NoProfile -NoLogo -ExecutionPolicy unrestricted -Command \"${environment};&.\\tools\\SetupTestEnvironment.ps1 Setup;&.\\tools\\update_schema.ps1;&.\\tools\\UpdateIISExpressCertificate.ps1;&.\\run.cmd -CI default-build /p:Configuration=${params.Configuration}\";"
    }
}
