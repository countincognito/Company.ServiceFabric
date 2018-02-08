# Company.ServiceFabric

This a minimalist template for building microservice architectures, using the [IDesign Method](http://www.idesign.net/), in Service Fabric using .NET Core 2.0, a HTTPS RESTful public API, and Swagger. It is heavily influenced by code samples that can be downloaded from [IDesign](http://www.idesign.net/Downloads).

It requires a local installation of [Seq](https://getseq.net/) for logging, and a minimum of version 3.0.456 of the [Service Fabric SDK](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started) and version 6.1.456 of the Service Fabric Runtime (Visual Studio 15.5.6). For further details see the [Service Fabric 6.1 release notes](https://msdnshared.blob.core.windows.net/media/2018/02/Microsoft-Azure-Service-Fabric-Release-Notes-SDK3.0-Runtime6.1.pdf).

The **Company.ServiceFabric.sln** solution includes all component, framework and configuration projects. The **Company.InProc.sln** solution includes only the component interfaces and implementations - this is to demonstrate just one possible way of separating business code from plumbing in order to make development and testing easier.

Be sure to update the **apiCertThumbprint** and the **seqLocation** parameters in the **ApplicationManifest.xml** configuration file with the appropriate values, and that all projects are set to compile to 64 bit.
