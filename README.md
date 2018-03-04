# SQL Server Analysis Services MDX Script Debugger
The MDX Script Debugger allows you to debug the performance of the MDX Script of your SQL Server Analysis Services cube. It takes any reference query and executes it against the target cube. It starts with a blank MDX script and basically adds a new command before each subsequent execution of the query. The output is the complete list of the single MDX script commands, their impact on the execution of the reference query and which commands had effectively been used for the query

## Supported Features:

* Check which SCOPES are effectively used by a given query
* Also works with leaf-level assignments to physical measures which are aggregated upwards
* Check performance of each assignment for a given query
* Check runtime vs. reference query and vs. previous statement
* Export results to XML
* Test custom MDX-Script without changing the cube
* Automatically move CREATE MEMBER statements on top of the MDX script 
* (useful if they are used in the reference query)
* Clear Cache support before execution
* Interactive Layout / Asynchronous execution

## Download: [gbrueckl.AnalysisServices.MdxScriptDebugger.zip](https://github.com/gbrueckl/AnalysisServices.MDXScriptDebugger/blob/master/Downloads/gbrueckl.AnalysisServices.MdxScriptDebugger.zip)

![Results View](http://files.gbrueckl.at/codeplex/mdxscriptdebugger/ResultsView.jpg "Results View")

![Custom MDX Script](http://files.gbrueckl.at/codeplex/mdxscriptdebugger/CustomScript.jpg "Custom MDX Script")

