# learnwsclient

Learnwsclient is a .NET, command-line web services client for Blackboard, Inc's Learning Management System, Learn(tm). It utilizes the raw algorithms found in the project [wsclient](https://github.com/allenvanderlinde/wsclient) and offers powerful look-up features similar to a database for users, courses, and enrollments.

Some features are:

* Customizable proxy tool registration with full entitlements
* Returns request data in Snapshot FlatFile format ready for modification and integration
* Exports into Flatfile format (.txt) or Comma-separated values format (.csv)

If you use the release version executable, there is something similar to a man page accessed by entering just "learnwsclient" in the command line.

If you are a developer or learner interested in reviewing or modifying learnwsclient's code base, please note that you very well may find a bug here or there or even optimizations I've missed. Please go ahead and fork this project, update it in any way you like, and share the knowledge!

I've refactored this code into a fully-featured application with a user interface and extra features. As that project progresses into newer releases, I'd like to provide some open-source aspects of it as well, found here on GitHub!

## Requirements

learnwsclient was written with Visual Studio 2015 and the necessary project/solution files are found within this repo. However, you'll have to build the BbWebServices.dll locally and reference Microsoft.Web.Services2.dll from within your own VS solution (or download Microsoft WSE 3.0).

Feel free to follow [this](https://community.blackboard.com/community/developers/blog/2016/10/25/getting-started-with-developing-a-net-learn-web-services-client) for details on the process.
