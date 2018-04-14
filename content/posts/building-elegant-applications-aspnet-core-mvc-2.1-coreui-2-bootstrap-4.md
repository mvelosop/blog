---
language: en
spanishVersion: /posts/construyendo-aplicaciones-elegantes-aspnet-mvc-core-2-bootstrap-4-coreui/
title: Building elegant applications with ASP.NET Core MVC 2.1 and CoreUI 2 (Bootstrap 4)
draft: false
author: Miguel Veloso
date: 2018-04-07
description: Adapt a BS 4 template to an MVC application, to enhance the user experience
thumbnail: posts/images/asp.net-core-mvc-coreui-en.png
tags: [ "User Experience", "Client Side Development", "Bootstrap 4", "CoreUI" ]
repoName: AspNetCore21CoreUI2
# repoRelease: "1.0"
toc: true
image:
    authorName: Benjamin Child
    url: https://unsplash.com/photos/0sT9YhNgSEs
---

This is the **UPDATED** and **REVISED** version of my previous post: [Building elegant applications with ASP.NET MVC Core 2 and Bootstrap 4 using CoreUI](/posts/building-elegant-applications-aspnet-mvc-core-2-bootstrap-4-coreui/)

In this post I'll explain how to adapt the [CoreUI](http://coreui.io/) template [(v 2.0.0 currently in beta)](https://github.com/coreui/coreui-free-bootstrap-admin-template/tree/v2.0.0), based on [Bootstrap 4](http://getbootstrap.com/), to use it as a base for ASP.NET MVC Core 2.1 applications.

Although this is ASP.NET Core MVC 2.1 specific, the adaptation procedure is quite similar for previous versions of ASP.NET MVC and should, at least, serve as a guide for other frameworks outside the .NET world.

> {{< IMPORTANT "Key Takeaways" >}}

> 0. Key concepts about handling client-side packages with **npm**

> 0. Understanding the role of [Gulp](https://gulpjs.com/) in client-side development

> 0. Understanding the relations between main views, layout views and partial views.

{{< repoUrl >}}

## Context

It's been a little while since I wrote the first post on CoreUI and as of these dates, there are new versions in preview for both ASP.NET MVC Core (v2.1) and CoreUI (v2.0.0) and I've also become a little more knowledgeable of the front-end, so I thought it would be a good time to publish an updated and revised post.

The process for adapting CoreUI is going to be a bit different from the previous post, on one side, it's now all centered in **npm**, as **bower** and **gulp** have been removed from both VS and CoreUI, CoreUI is using npm's task execution capabilities and, on the other side, I expect to make the process much clearer.

**As of today (08-APR-2018), ASP.NET Core 2.1 is only supported by Visual Studio 2017 15.7 Preview 2.**

I don't expect this post to be too affected by the final release of all involved products, but I'll update it in case it's necessary.

### Platform and Tools

* [Visual Studio 2017 Community Edition (Preview 3)](https://www.visualstudio.com/thank-you-downloading-visual-studio/?ch=pre&sku=Community&rel=15)  
(go to [Visual Studio's download page](https://www.visualstudio.com/downloads/) for other versions).

* [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

* [CoreUI v2.0.0 (beta)](https://github.com/coreui/coreui-free-bootstrap-admin-template)

* [.NET Core 2.1.0-Preview1 with SDK  2.1.300-Preview1 - x64 SDK Installer](https://download.microsoft.com/download/D/7/8/D788D3CD-44C4-487D-829B-413E914FB1C3/dotnet-sdk-2.1.300-preview1-008174-win-x64.exe)  
(go to [.NET Core's download page](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) for other versions).

* [Node 9.11.1](https://nodejs.org/)

* [npm 5.8.0](https://www.npmjs.com/)

The adaptation process becomes much easier using a file compare tool. I recommend Beyond Compare, that I've been using since 1998, but there are other similar tools that should work just as well.

* [Beyond Compare, from Scooter Software](http://www.scootersoftware.com/)

## Step by step

### 1 - Create an ASP.NET MVC Core 2.1 project

Let's start by creating a standard MVC application, using Visual Studio 2017's built-in template.

#### 1.1 - Create a blank solution

1. Create a "blank solution" named "**AspNetCore21CoreUI2**" with a Git repository

2. Add a "**src**" solution folder

3. Add a "**Solution Items**" solution folder

4. Add file "**.editorconfig**" to standarize some formatting options for the project

{{<renderSourceFile ".editorconfig">}}

Right now your solution should look like this:

{{<image src="/posts/images/devenv_2018-04-08_14-15-24.png">}}

#### 1.2 - Add an ASP.NET MVC Core 2.1 project

1. Create the **CoreUI.Mvc** project of type "**ASP.NET Core Web Application**" in the "**src**" solution folder and also create the "**src**" folder in the file system.
{{<image src="/posts/images/devenv_2018-04-08_14-25-47.png">}}
and upon browsing for the folder, create the "**src**" folder in the solution folder
{{<image src="/posts/images/2018-04-08_14-31-44.png">}}

2. Select an **ASP.NET Core 2.1** **MVC** type application and 
{{<image src="/posts/images/devenv_2018-04-08_14-37-27.png">}}

3. Change authentication to **Individual User Accounts** and **Store user accounts in-app**
{{<image src="/posts/images/devenv_2018-04-08_14-40-18.png">}}

Right now your solution should look like this:
{{<image src="/posts/images/devenv_2018-04-08_14-46-42.png">}}

#### 1.3 - Create the database

1. Change the connection string in `appsettings.json` file to work with SQL Server Developer Edition instead on the default LocalDb
{{<renderSourceCode "text" "linenos=table">}}
Server=localhost; Initial Catalog=CoreUI.Mvc; Trusted_Connection=True; MultipleActiveResultSets=true
{{</renderSourceCode>}}

2. Run the application using [Ctrl]+[F5]

3. Sign up to force database creation

4. Click **Apply Migrations**, when you get the database missing error:
{{<image src="/posts/images/chrome_2018-04-08_14-56-23.png">}}

5. Refresh the screen when the database creation process is complete, to finish user registration
{{<image src="/posts/images/chrome_2018-04-08_14-58-08.png">}}

**This is a good time to save the project in your repo!**

#### 1.4 - Delete original client side libraries

The "**wwwroot**" folder is the root of the application's client-side, so all client-side static files should be within this folder tree.

We're now going to remove all the client-side libraries included by the Visual Studio template, because we're going to use CoreUI's ones.

1. Open the wwwroot folder and make a note of the libraries used:
{{<image src="/posts/images/devenv_2018-04-08_15-13-44.png">}}
The boostrap folder contains all of the "look" and general "feel" of the application, but this is what's going to be replaced by CoreUI, so we'll just ignore this folder completely. <br /><br />
The "**jquery***" libraries are used for client side interactivity and validation, so we will later add them to the CoreUI site, but we'll just take a note of the libraries used, we can check the versions in the "**.bower.json**" file in each folder, in this case:
    - jquery (2.2.0)
    - jquery-validation (1.14.0)
    - jquery-validation-unobtrusive (3.2.6)

2. Delete the "**wwwroot\lib**" folder.<br /><br />
If you can't delete the "**lib**" folder, you might need to close VS and do it from the file explorer. 
 
3. Run the application with [Ctrl]+[F5] to display it without any style (or Javascript)
{{<image src="/posts/images/chrome_2018-04-08_15-40-50.png">}}

**Let's save this version to the repository now**

### 2 - Prepare the CoreUI deployment site

We're now going to prepare a deployment (distribution) site from the latest version of CoreUI, which we'll later copy to our ASP.NET MVC Core application.

#### 2.1 - Prepare your base CoreUI repository

Clone the [CoreUI repository in GitHub](https://github.com/coreui/coreui-free-bootstrap-admin-template) to any folder you like, outside the solution.

{{<renderSourceCode "bat" "linenos=table">}}
git clone https://github.com/coreui/coreui-free-bootstrap-admin-template
{{</renderSourceCode>}}

You could also fork it in GitHub and then clone your repo, so you can then push your customizations and, eventually, contribute to the CoreUI repo.

We'll now create a new branch to do our customizations, let's call it "**dist**", so:

{{<renderSourceCode "bat" "linenos=table">}}
cd .\coreui-free-bootstrap-admin-template
git checkout -b dist master
{{</renderSourceCode>}}

#### 2.2 - Create the distribution folder

First we need to install client-side dependencies, so, from the **coreui-free-bootstrap-admin-template** folder, execute this command:

{{<renderSourceCode "bat" "linenos=table">}}
npm install
{{</renderSourceCode>}}

That creates the "**node_modules**" folder with all dependencies, as configured in the "**dependencies**" collection in the "**package.json**" file. 

Execute this command to verify the site is working properly:

{{<renderSourceCode "bat" "linenos=table">}}
npm run serve
{{</renderSourceCode>}}

That should open your default browser at http://localhost:3000/ where you should see something like this:
{{<image src="/posts/images/chrome_2018-04-08_19-54-45.png">}}

To finally generate the distribution folder we just need to execute the command:

{{<renderSourceCode "bat" "linenos=table">}}
npm run build
{{</renderSourceCode>}}

That creates the "**dist**" folder (.git-ignored) inside the repo folder with something like this:
{{<image src="/posts/images/explorer_2018-04-09_13-47-00.png">}}

This is the base static deployment site, ready to be published, and by just double-clicking any html file (except 404 or 500) you can explore the base CoreUI template.

#### 2.3 - Install the dependencies for client-side ASP.NET MVC views

We will now install the dependencies identified in [1.4](#1-4-delete-original-client-side-libraries).

We might have to use with [Ctrl]+[C] to interrupt the **`npm run serve`** from the last step.

To install the dependencies we just have to add these lines to the "**dependencies**" collection on "**packages.json**", referencing the lastest versions of the packages:

{{<renderSourceCode "json" "">}}
"jquery-validation":"^1.17.0",
"jquery-validation-unobtrusive":"^3.2.9",
{{</renderSourceCode>}}

So the file should result in something like this:
{{<image src="/posts/images/Code_2018-04-09_13-09-42.png">}}

Then, we have to install the new dependencies using:

{{<renderSourceCode "bat" "linenos=table">}}
npm install
{{</renderSourceCode>}}

Now we have to create a "**vendors.html**" file, referencing the files we want to copy to the "**dist/vendors**" folder, like this:

{{<renderSourceCode "html" "linenos=table">}}
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8" />
  <title>Vendor list</title>
</head>

<body style="font-family: Arial, Helvetica, sans-serif; font-size: 18px;">

<h3>Files to deploy to dist/vendors</h3>
<ul>
  <li>node_modules/jquery-validation/dist/jquery.validate.min.js</li>
  <li>node_modules/jquery-validation/dist/additional-methods.js</li>
  <li>node_modules/jquery-validation/dist/localization/messages_es.js</li>
  <li>node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js</li>
</ul>

</body>

</html>
{{</renderSourceCode>}}

This file will be scanned during the "**build**" command to select the files that will be copied from "**node_modules**" to "**dist/vendors**".

So we can now create/update the distribution folder with:

{{<renderSourceCode "bat" "linenos=table">}}
npm run build
{{</renderSourceCode>}}

> {{< IMPORTANT "Client-side package managers" >}}.

> - Client-side packages should be handled by some packager manager, but we won't get cover that in this post.

#### 2.4 - Copy the distribution folder into the solution

Now we'll just copy the contents of the "**dist**" folder into the new "**src\CoreUI**" folder of our solution.

The "**src\CoreUI**" will be the reference folder we'll use to compare to new versions of CoreUI, when they become available, to update the components in our application as needed.

The solution should now be like this:

{{<image src="/posts/images/explorer_2018-04-09_18-07-30.png">}}

In a moment we will  merge that "**dist**" folder to our app's wwwroot and, to make it cleaner, we'll rename the "**images**" folder to "**img**", just like CoreUI's equivalent.

**This is an excellent time to commit to your local repo!**

> {{< IMPORTANT "src\CoreUI is not part of Visual Studio's solution" >}}.

> 0. Notice that, although src\CoreUI is within the solution's folder structure and under source control, it's not part of the Visual Studio solution, i.e. you will not see it in the solution explorer.

### 3 - Integrate the distribution folder into the MVC application

In the integration process we separate files in two sets:

1. Html files, that have to be converted to **Razor** views and 
2. All other (static) files, that will just be copied to the "**wwwroot**" folder

This is where a tool like Beyond Compare really shines, specially when it's time to update the files to new versions of CoreUI.

#### 3.1 - Copy static files to wwwroot folder

So we have to copy these folders from "**src\CoreUI**" to "**src\CoreUI.Mvc\wwwroot**":

- css
- img
- js
- vendors

If using [Beyond Compare](http://www.scootersoftware.com/), the result should be something like this:

{{<image src="/posts/images/BCompare_2018-04-10_10-55-47.png">}}


#### 3.2 - Create a generic controller for CoreUI views

Next we'll convert the static CoreUI-s html files to Razor views (*.cshtml), but first need to create a generic controller for CoreUI's views, that just receives the name of the view to display and returns it.

{{<renderSourceFile "src\CoreUI.Mvc\Controllers\CoreUIController.cs">}} 

We also need the corresponding "**Views\CoreUI**" folder for the Razor views.

#### 3.3 - Create the initial Razor _Layout view

When working in Razor, the rendered page usually has, at least, two parts:

1. The **content**, which is the core of whatever you want to show at any one time, e.g. the Index page, and can be named whatever makes sense, i.e. "**Index.cshtml**".

2. The **layout**, that "surrounds" the content and is usually named "**_Layout.cshtml**".

Making the equivalence to CoreUI's html files, it's easy to realize that for the initial Index view:

- The layout is equivalent to "**blank.html**"
- The content is equivalent to the difference between "**index.html**" and "**blank.html**"

So first, let's compare the "**src\CoreUI**" to "**src\CoreUI.Mvc\Views\CoreUI**" folders and hide the folders we've already copied to wwwroot:

{{<image src="/posts/images/2018-04-10_11-13-19.png">}}

And now:

1. Copy "**blank.html**" to the right side and rename it to "**_Layout.cshtml**"

2. Create an empty "**Index.cshtml**" file on the right side

You should have this now:

{{<image src="/posts/images/BCompare_2018-04-10_11-51-25.png">}}

Then we have to edit the "**_Layout.cshtml**" file and make these changes:

1. Disable the default layout by adding `@{ Layout = ""; }` before the html code.

2. Replace **`@`** with **`@@`** to avoid Razor sintax error.

3. Add **`~/`** to all references to static files in wwwroot.

4. Add **`@ RenderBody()`** where the page content should be.

To get to this:

{{<image src="/posts/images/devenv_2018-04-10_16-38-26.png">}}

{{<image src="/posts/images/devenv_2018-04-10_16-45-25.png">}}

We can now execute the application with [Ctrl]+[F5] and when navigating to https://localhost:#####/CoreUI/Index (##### = port assigned by VS) we should get this:

{{<image src="/posts/images/chrome_2018-04-10_17-09-40.png">}}

You should check the "**Network**" section on the browser's developer tools to fix all missing references:

{{<image src="/posts/images/chrome_2018-04-10_17-06-07.png">}}


> {{< IMPORTANT "Razor Views and html" >}}

> 0. Any valid .html file is also a valid Razor view, you should only need to change the extension to .cshtml and replace @ for @@.

#### 3.4 - Create the Razor Index view

Using [Beyond Compare](http://www.scootersoftware.com/) (or equivalent) to compare "**src\CoreUI**" to "**src\CoreUI.Mvc\Views\CoreUI**":

1. Delete the "**Index.cshtml**" file in "**Views\CoreUI**"

2. Copy "**index.html**" to the right side

3. Compare "**blank.html**" (left side) to "**index.html**" (right side)

{{<image src="/posts/images/BCompare_2018-04-10_17-56-49.png">}}

On the left thumbnail view we can identify four zones:

1. Before the index content zone both files are identical
2. In the index content zone the **blank.html** has no lines
3. Afert the index content zone bot files are identical again
4. There are a few additional lines at the end of the **index.html** file

So we just delete the common lines from zones 1, 2 and the last two lines on the **index.html** file (on the right side), by selecting them in Beyond Compare and pressing the [Del] key:

When finishing we should get to this:

{{<image src="/posts/images/BCompare_2018-04-10_20-31-31.png">}}

Notice the four lines at the end that exist only in **index.html**.

The way to handle that is creating a **Scripts** section in the **index.html** file and adding a call to the **RenderSection** helper in the **_Layout**.

So we have to:

1. Rename the "**index.html**" file to "**Index.cshtml**"

2. Enclose the script lines in a Scripts section and

3. Fix the references prepending **`~/`**, just like we did on the layout before

After that, the last lines of "**Index.cshtml**" should be:

{{<image src="/posts/images/devenv_2018-04-10_21-20-25.png">}}

And now the last lines of "**_Layout.cshtml**" should be:

{{<image src="/posts/images/devenv_2018-04-10_21-46-39.png">}}

So, if we now run the application again and refresh the https://localhost:#####/CoreUI/Index page, we should get this:

{{<image src="/posts/images/chrome_2018-04-10_21-34-56.png">}}

#### 3.5 - Componentize the _Layout view

It's not practical to have a massive 700+ lines _Layout view, so we'll split it into smaller components.

We're not going through the whole process here, will only show the final result and you can check all details in the post's repo, but it should be pretty obvious:

{{<renderSourceCode "html" "linenos=table">}}
@{ Layout = ""; }

<!--
* CoreUI - Free Bootstrap Admin Template
* @@version v2.0.0-beta.0
* @@link https://coreui.io
* Copyright (c) 2018 creativeLabs Łukasz Holeczek
* Licensed under MIT (https://coreui.io/license)
*
* Adaptated to ASP.NET MVC Core 2.1 by Miguel Veloso
* http://coderepo.blog
-->
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, shrink-to-fit=no">
    <meta name="description" content="CoreUI - Open Source Bootstrap Admin Template">
    <meta name="author" content="Łukasz Holeczek">
    <meta name="keyword" content="Bootstrap,Admin,Template,Open,Source,jQuery,CSS,HTML,RWD,Dashboard">
    <!-- Icons-->
    <link href="~/vendors/css/flag-icon.min.css" rel="stylesheet">
    <link href="~/vendors/css/font-awesome.min.css" rel="stylesheet">
    <link href="~/vendors/css/simple-line-icons.css" rel="stylesheet">
    <!-- Main styles for this application-->
    <link href="~/css/style.css" rel="stylesheet">
</head>
<body class="app header-fixed sidebar-fixed aside-menu-fixed sidebar-lg-show">

    <!-- *APP-HEADER* -->
    @Html.Partial("_AppHeader")
    <!-- /*APP-HEADER* -->

    <div class="app-body">
        <div class="sidebar">

            <!-- *SIDE-BAR-NAV* -->
            @Html.Partial("_SideBarNav")
            <!-- /*SIDE-BAR-NAV* -->
            <button class="sidebar-minimizer brand-minimizer" type="button"></button>
        </div>

        <main class="main">

            <ol class="breadcrumb">

                <!-- *BREADCRUMB* -->
                @Html.Partial("_Breadcrumb")
                <!-- /*BREADCRUMB* -->

                <!-- *BREADCRUMB-MENU* -->
                @Html.Partial("_BreadcrumbMenu")
                <!-- /*BREADCRUMB-MENU* -->

            </ol>

            <div class="container-fluid">
                <div class="animated fadeIn">

                    <!-- *CONTENT* -->
                    @RenderBody()
                    <!-- /*CONTENT* -->

                </div>
            </div>

        </main>

        <!-- *ASIDE-MENU* -->
        @Html.Partial("_AsideMenu")
        <!-- /*ASIDE-MENU* -->

    </div>

    <footer class="app-footer">
        <div>
            <a href="https://coreui.io">CoreUI</a>
            <span>&copy; 2018 creativeLabs.</span>
        </div>
        <div class="ml-auto">
            <span>Powered by</span>
            <a href="https://coreui.io">CoreUI</a>
        </div>
    </footer>

    <!-- Bootstrap and necessary plugins-->
    <script src="~/vendors/js/jquery.min.js"></script>
    <script src="~/vendors/js/popper.min.js"></script>
    <script src="~/vendors/js/bootstrap.min.js"></script>
    <script src="~/vendors/js/pace.min.js"></script>
    <script src="~/vendors/js/perfect-scrollbar.min.js"></script>
    <script src="~/vendors/js/coreui.min.js"></script>

    <!-- Plugins and scripts required by the views -->
    @RenderSection("Scripts", required: false)

</body>
</html>{{</renderSourceCode>}}

And the resulting files:

{{<image src="/posts/images/devenv_2018-04-10_23-38-32.png">}}

If we now go back to https://localhost:#####/CoreUI/Index, we'll see the same view, but this time as a composition of the main content on the layout and the partial views.

#### 3.6 - Convert the rest of the CoreUI pages

To convert the rest of the pages into razor views, we just have to repeat step 3.4 for each html file in "**src\CoreUI**". You can repeat the process as many times as you want, to help consolidate the process and can get the rest of the files, or compare them, from the post's repo.

Besides splitting, we also have to **change the links to reference the corresponding route using Tag Helpers**, so they use **CoreUIController**.


### 4 - Integrate CoreUI views with MVC application

Finally, what's left is just:

1. Move the _Layout and all partial views to the Shared folder
2. Modify links to CoreUI pages to use the controller
3. Transfer the functionality of _LoginPartial.cshtml to _UserNav.cshtml
4. Include the handling of styles and scripts per view in _Layout.cshtml 
5. Adapting the Carousel to Bootstrap 4

plus some other minor detail that's best seen in the repository.

With this, we finally reached our goal:

{{<image src="/posts/images/chrome_2017-11-03_23-29-54.png">}}

* The MVC application menu in the top navigation bar
* The CoreUI menu in the side navigation pane
* The user menu from the profile photo, with functional "Profile" and "Logout" options

In short, I think a good starting point for an elegant and attractive user interface in your next project.

## Summary

In this article we looked at the process in quite detail of adapting a static HTML template to ease the development of attractive ASP.NET MVC applications.

In doing so, we learned a little more about the structure and use of client-side packages for user interface development.

---

{{< goodbye >}}

---

### Related links

**Bootstrap 4**<br/>
https://getbootstrap.com/docs/4.0/getting-started/introduction/

**CoreUI**<br/>
http://coreui.io/

**CoreUI en GitHub**<br/>
https://github.com/mrholek/CoreUI-Free-Bootstrap-Admin-Template

**Gulp**<br/>
https://gulpjs.com/

**Installing CoreUI-s static version**<br/>
http://coreui.io/docs/getting-started/static-version/

**Node**<br/>
https://nodejs.org

**npm**<br/>
https://www.npmjs.com/

**Resharper**<br/>
https://www.jetbrains.com/resharper/
