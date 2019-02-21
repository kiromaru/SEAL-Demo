# Cloud Functions Demo

This demo shows how to use Microsoft SEAL for .NET to perform homomorphic operations in Azure Functions.

Azure Functions is a solution for easily running small pieces of code, or "functions", in the cloud. You can
write just the code you need for the problem at hand, without worrying about a whole application or the
infrastructure to run it. Functions can make development even more productive, and you can use your programming
language of choice, such as C#, F#, Node.js, Java, or PHP. You pay only for the time your code runs and trust 
Azure to scale as needed. Azure Functions lets you develop serverless applications on Microsoft Azure. Learn more 
by [following this link](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview).

The demonstration implements basic matrix operations: addition, subtraction, and multiplication.
One of the projects (`CloudBasedFunctions/CloudBasedFunctions.sln`) implements these operations in Azure Functions, 
receiving as input a pair of ciphertexts. The other project (`ClientBasedFunctions/ClientBasedFunctions.sln`) 
provides a user interface that allows entering matrices, encrypting them, sending them to the cloud functions, 
receiving the result, decrypting the result, and finally showing it to the user. The user interface has an option 
to show the actual data that is being sent and received from the cloud functions.

Matrix addition and subtraction is implemented by converting the matrices to vectors (using `BatchEncoder` class in Microsoft SEAL), 
and then performing element-wise operations in the resulting ciphertexts. Matrix multiplication is slightly
more complicated and is explained in detail in [this file](ClientBasedFunctions/ClientBasedFunctions/MatrixProduct.md).


## Instructions for Cloud-Based Implementation

### Steps to configure project to run Azure Functions locally

1.  [Download 64-bit Azure Functions Command Line Interface](https://github.com/Azure/azure-functions-core-tools/releases).
2.  Install to known directory, for example: `D:\Progs\Azure.Functions.Cli.win-x64.2.3.199`.
3.  Open CloudBasedFunctions solution.
4.  Right click on CloudBasedFunctions project and select `Properties`.
5.  Go to `Debug` tab in project properties.
6.  In `Executable`, browse for `func.exe` on the 64-bit Azure Functions Command Line download, for example: `D:\Progs\Azure.Functions.Cli.win-x64.2.3.199\func.exe`.
7.  In `Application arguments`, add: `host start`.
8.  In `Working directory`, type `$(TargetDir)`.
9.  Now you should be able to debug the CloudBasedFunctions project locally.

### Steps to configure project to run Azure Functions in Azure

1.  Open CloudBasedFunctions solution.
2.  Right click on CloudBasedFunctions project and select `Publish...`.
3.  You will be asked to select a publish target, select Azure Function App.
4.  After clicking `Publish` you might have to enter your Azure credentials.
5.  Once the functions are published, go to the [Azure Portal](https://portal.azure.com).
6.  Select `Function Apps`.
7.  Select your newly published Function App.
8.  Select `Platform features` tab.
9.  Under `General Settings`, select `Application Settings`.
10. Change the `Platform` to 64 bit, save your changes.
11. After this change, Functions should be enabled and running.


## Instructions for Client-Based Implementation

### Steps to configure client project to connect to locally run Azure Functions

1.  Open CloudBasedFunctions solution.
2.  Start CloudBasedFnctions project, functions should run locally. A command
    window will appear and after some initialization the available Azure Functions
    will be printed, as well as the http address where they are available, for
    example: `http://localhost:7071/api/Addition`.
3.  Open ClientBasedFunctions solution.
3.  Start ClientBasedFunctions project.
4.  The app window will appear. At the top there is a text box to specify the base
    address of the Azure Functions. To connect to the Azure Functions in step 2 above,
    for example, the textbox should contain: `http://localhost:7071`.

### Steps to configure client project to connect to Azure Functions in Azure

1.  Please follow the instructions in the CloudBasedFunctions solution to deploy the
    functions to Azure..
2.  Go to [Azure Portal](https://portal.azure.com).
3.  Select "Function Apps".
4.  Select your Function App.
5.  In the right part of the screen you will see the URL you need to use to access
    the Azure Functions, for example: `https://sealazurefuncdemoXXXX.azurewebsites.net`.
6.  In the left part of the screen you will see a tree structure. The root node is
    your Function App, its first child node is `Functions`, and below this your will
    find the `Addition`, `Subtraction` and `Multiplication` functions.
7.  For each function we need to obtain the function key needed to be able to call
    the API. When you select a function in the tree, the right part of the screen
    will show the contents of the `function.json` file that configures the function.
    At the top you will see a link called `</> Get function URL`.
8.  Clicking `</> Get function URL` will show a dialog showing the complete URL
    needed to access the function. Make sure `default (Function key)` is selected in
    the combo box, and copy only the value of the function key. This is the text
    after `?code=`.
9.  Open ClientBasedFunctions solution.
10. In the ClientBasedFunctions project, open the file `GlobalProperties.cs`.
11. Update the constants under `GlobalProperties.Codes` with the function keys from
    your Function App.
12. Run the ClientBasedFunctions project, and in the top textbox enter the URL from
    step 4, for example: `https://sealazurefuncdemoXXXX.azurewebsites.net`.
13. This will enable the client to connect to the Azure Functions running in Azure.
