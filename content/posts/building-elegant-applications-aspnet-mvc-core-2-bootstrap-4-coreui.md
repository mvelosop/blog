---
language: en
spanishVersion: /posts/construyendo-aplicaciones-elegantes-aspnet-mvc-core-2-bootstrap-4-coreui/
title: Building elegant applications with ASP.NET MVC Core 2 and Bootstrap 4 using CoreUI
draft: false
author: Miguel Veloso
date: 2018-02-01
description: Adapt a BS 4 template to an MVC application, to enhance the user experience
thumbnail: posts/images/benjamin-child-90768.jpg
tags: [ "User Experience", "Client Side Development", "Bootstrap 4", "CoreUI" ]
repoName: AspNetCore2CoreUI
repoRelease: "1.0"
toc: true
image:
    authorName: Benjamin Child
    url: https://unsplash.com/photos/0sT9YhNgSEs
---

In this post we'll explain how to adapt the [CoreUI](http://coreui.io/) template, based on [Bootstrap 4](http://getbootstrap.com/), to use it as a base for ASP.NET MVC Core 2 applications.

> {{< IMPORTANT "Key Takeaways" >}}

> 0. Key concepts about handling client-side packages with **npm**

> 0. Understanding the role of [Gulp](https://gulpjs.com/) in client-side development

> 0. Understanding the relations between main views, layout views and partial views.

{{< repoUrl >}}

## Context

I usually work on the back-end, but there's always some user interface work to do, and since I'm not too good at design, I decided to search for modern templates based on [Bootstrap 4](https://getbootstrap.com/docs/4.0/getting-started/introduction/) to adopt one.

There are many options available in this field, just search for admin dashboard templates in [Google](https://www.google.es/search?q=admin+dashboard+templates) and you'll find many free and low-cost options (US$ 10-30). You can pick just about anyone to get a pretty good looking user interface (at least from my point of view).

In fact, I have come to buy a couple of these templates, but, although they do comply with the look and feel aspect, looking at their internal structure and the amount of Javascript they use, I've been a bit disappointed.

So, leveraging that I recently finished a project where I used a template, I resumed the search and found [CoreUI](http://coreui.io/), which is open source, has "clean internals", is based on [Bootstrap 4](https://getbootstrap.com/docs/4.0/getting-started/introduction/) and has a "pro" version, with many things already resolved for a good price, especially considering the time I would need to get something like that.

In this article I will focus on the process of adapting the static HTML 5 version of [CoreUI (OpenSource)](http://coreui.io/) by downloading it directly from [GitHub](https://github.com/mrholek/CoreUI-Free-Bootstrap-Admin-Template), replacing the HTML components of a basic ASP.NET MVC Core 2 application generated directly from Visual Studio 2017.

I hope to do this in a way that will make it easy to update the base project when [Lukas Holeczek](https://about.me/lukaszholeczek) publishes new versions of the template.

### Platform and Tools

* [Visual Studio 2017 Community Edition](https://www.visualstudio.com/es/thank-you-downloading-visual-studio/?sku=Community&rel=15)  
(go to [Visual Studio's download page](https://www.visualstudio.com/es/downloads/) for other versions).

* [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

* [.NET Core SDK 2.0.2 with .NET Core 2.0.0 - x64 SDK Installer](https://download.microsoft.com/download/7/3/A/73A3E4DC-F019-47D1-9951-0453676E059B/dotnet-sdk-2.0.2-win-x64.exe)  
(go to [.NET Core's download page](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md) for other versions).

## Step by step

### 1 - Create an ASP.NET MVC Core 2 project

Let's start by creating a standard MVC application, using Visual Studio 2017's built-in template.

#### 1.1 - Create a blank solution

1. Create "blank solution" "AspNetCore2CoreUI" with a Git repository

2. Add "src" solution folder

3. Right now your solution should look like this:

{{<image src="/posts/images/devenv_2017-10-31_17-06-44.png">}}

#### 1.2 - Add an ASP.NET MVC Core 2 project

1. Create **CoreUI.Web** project of type "ASP.NET Core Web Application" in "src" solution folder and also create "src" folder in the file system.
{{<image src="/posts/images/devenv_2017-10-31_17-13-01.png">}}
and select that folder to create the project
{{<image src="/posts/images/2017-10-31_17-22-03.png">}}

2. Select an **ASP.NET Core 2.0** **MVC** type application and 
{{<image src="/posts/images/devenv_2017-10-31_17-30-23.png">}}

3. Change authentication to **Individual User Accounts** and **Store user accounts in-app**
{{<image src="/posts/images/devenv_2017-10-31_17-38-40.png">}}

#### 1.3 - Create the database

1. Change the connection string in `appsettings.json` file to work with SQL Server Developer Edition
`Server=localhost; Initial Catalog=CoreUI.Web; Trusted_Connection=True; MultipleActiveResultSets=true`

2. Run the application using [Ctrl]+[F5]

3. Sign up to force database creation

4. Click **Apply Migrations**, when you get the database missing error:
{{<image src="/posts/images/chrome_2017-10-31_18-02-35.png">}}

5. Refresh the screen when the database creation process is complete, to finish user registration
{{<image src="/posts/images/chrome_2017-10-31_18-05-42.png">}}

**This is a good time to save the project in your repo!**

#### 1.4 - Delete original client side libraries

We're now going to remove all the client-side libraries included by the Visual Studio template, because we're going to use CoreUI's ones.

We're also going to remove Bower as a client-side package manager, because we'll be using [npm](https://www.npmjs.com/) and [Node](https://nodejs.org) as part of CoreUI and [npm](https://www.npmjs.com/) can get us all the packages we'll need.

And so we get to know a little more about client-side.

Let's get started:

1. Write down the client-side dependencies (in the bower.json file)
{{<image src="/posts/images/devenv_2017-11-01_13-14-56.png">}}

2. Delete the bower.json file

3. Close the solution and reopen it to remove the "bower" folder from the project dependencies.

4. Open the wwwroot folder and delete the "lib" folder
{{<image src="/posts/images/devenv_2017-11-01_14-50-59.png">}}

5. Run the application with [Ctrl]+[F5] to display it without any style (or Javascript)
{{<image src="/posts/images/chrome_2017-11-01_14-54-42.png">}}

**Let's save this version to the repository now**

### 2 - Prepare base site

We're now going to prepare the base folder with CoreUI, which we'll use to copy the components to our MVC application.

In this process we will learn something (or at least I learned something) about the management of client-side libraries in Javascript.

#### 2.1 - Clone the CoreUI repository

Clone the [CoreUI repository in GitHub](https://github.com/mrholek/CoreUI-Free-Bootstrap-Admin-Template) to any folder you like, outside the solution.

#### 2.2 - Copy the HTML 5 static version into the solution

We are going to copy all the contents of the "**Static_Full_Project_GULP**" folder from the repository into the "**src\CoreUI**" solution's folder, outside the web project.

Initially I did this within the CoreUI.Web project, but Visual Studio got extremely slow after downloading the client-side packages in the **node_modules** folder, I'm not sure if it was because of Visual Studio itself or because of [Resharper](https://www.jetbrains.com/resharper/), but doing it outside the web project  worked like a charm.

We'll include a step later on to copy the final "deployment-optimized" version of CoreUI into the web project.

At this point the solution should look like this from the file system:
{{<image src="/posts/images/explorer_2017-11-03_11-33-21.png">}}

> {{< IMPORTANT "src\CoreUI is not part of Visual Studio's solution" >}}.

> 0. Notice that, although src\CoreUI is within the solution's folder structure and under source control, it's not part of the Visual Studio solution, i.e. you just don't see it in the solution explorer.

#### 2.3 - Install the required client-side packages

1. First we edit the **packages.json** file to include the most recent versions of the packages used by the Visual Studio template, the ones we wrote down in [1.4](#1-4-delete-original-client-side-libraries) and 

2. Then remove **gulp-bower-src** since we are not using Bower.

{{<image src="/posts/images/Code_2017-11-03_13-12-37.png">}}

Then we run the steps for [installing the static version of CoreUI](http://coreui.io/docs/getting-started/static-version/), except for the Bower installation step, from the command line interface in the **src\CoreUI** solution folder.

When we're done, the site (running with `gulp serve`) should look like this:
{{<image src="/posts/images/chrome_2017-11-03_12-53-00.png">}}

Take note of the **node_modules** folder, that contains the client-side packages specified in packages.json, and their dependencies as well.

{{<image src="/posts/images/explorer_2017-11-03_13-19-44.png">}}

> {{< IMPORTANT "Installing packages with npm" >}}.

> 0. When installing packages with **npm**, the **node_modules** folder is created with all the required components, both for development tools such as [Gulp](https://gulpjs.com/), and for running the application. This folder usually takes up a lot of space and should not be included in the application deployment.

> 0. The **package.json** file determines which packages will be installed when you run **npm install**.

#### 2.4 - Prepare the base "distribution" (deployment) version

All html files we copy from CoreUI's repo reference directly the files in **node_modules**, but these folders are development resources, not deployment resources.

To prepare the deployment-optimized version, let's run the `gulp build: dist` command from the command prompt on the **src\CoreUI** folder.

This will generate a new **dist** folder in **src\CoreUI** with the content ready for deployment, referring to the minimized versions of the required libraries.

{{<image src="/posts/images/explorer_2017-11-03_14-43-37.png">}}

If we click on the **src\CoureUI\dist\index.html** file we'll see that all references to **node_modules** were changed to **vendors** and all required components were unified in folders **css**, **fonts** and **js** inside **vendors**.

{{<image src="/posts/images/Code_2017-11-03_15-08-59.png">}}

#### 2.5 - Prepare the "distribution" version to integrate with the ASP. NET MVC project

Now let's make some minor adjustments to start the integration process with our ASP. NET MVC project.

First, we'll include the required validation libraries, modifying the **src\CoreUI\gulp-tasks\build-dist.js** file.

This file is used by [Gulp] (https://gulpjs.com/), a tool for automating tasks, that's frequently used in client-side development.

Then, let's include the line: `node_modules/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js`  
to have this library copied to the deployment version.

{{<image src="/posts/images/Code_2017-11-03_15-19-33.png">}}

Besides that, we're going to:

1. Create an additional task in the same file, to copy the distribution files to the **wwwroot** folder of our MVC project, where we'll be using them and

2. Include that task as part of `build: dist`

{{<image src="/posts/images/Code_2017-11-03_15-34-56.png">}}

> {{< IMPORTANT "Deployment Packages" >}}

> 0. The **vendorJS**, **vendorCSS** and **vendorFonts** lists from **build-dist.js** have the components used by the PRO version of CoreUI, but since they are not in **package.json**, they are not included for the deployment.

> 0. Packages usually have a **dist** folder that contains the components optimized for deployment and it's the developer's responsibility to include the correct files in those lists.

After this we run `gulp build: dist` again and if we now run the MVC application using [Ctrl]+[F5] and go to the address https://localhost:#####/index.html (##### = port assigned by VS) we should get something like this:

{{<image src="/posts/images/chrome_2017-11-03_15-40-22.png">}}

The same page as above but displayed as a static page within our MVC application.

**We will now save the solution in the repository.**

#### 2.6 - Including new components

While doing this, we also understood how to include new client-side components, such as a date-picker, in the user interface:

> {{< IMPORTANT "Steps to include new client-side components" >}}

> 1. Include a reference the library in **src\CoreUI|package. json**

> 2. Run **npm install** to download the package from the **npm** repository

> 2. Edit the **src\CoreUI\gulp-tasks\build-dist.js** file to update the list of files to be copied, in case they are not included.

> 3. Run `gulp build:dist` to generate the distribution version.

After that, you just need to include the new references (scripts, styles, images, etc.) in the Razor views as needed.

### 3 - Develop Razor version of CoreUI pages

In this section we'll convert the static HTML pages from CoreUI into Razor views (.cshtml) that can be used in any application.

#### 3.1 - Develop a generic controller for CoreUI views

This is a very simple controller, that receives the name of the view to display and returns it.

{{<renderSourceFile "src\CoreUI.Web\Controllers\CoreUIController.cs">}}

#### 3.2 - Create the initial Index view

To do this simply:

1. Copy page **index. html** from **wwwroot** to the new folder **Views\CoreUI**

2. Change the extension to .cshtlm

3. Remove the use of the standard _Layout by typing this at the top of the view:
```cs
@{
        Layout = "";
}
```
4. We changed all occurences of "@" to "@@" to avoid Razor's syntax error.

> {{< IMPORTANT "Razor Views and html" >}}

> 0. Any valid .html file is also a valid Razor view, you only need to change the extension to .cshtml.

Then, when running the application with [Ctrl]+[F5] and navigating to https://localhost:#####/CoreUI/Index (##### = port assigned by VS) we should see the following:

{{<image src="/posts/images/chrome_2017-11-03_17-37-33.png">}}

This happens because references to css, js, images, etc. files need to be corrected.

This is all it takes:

1. Identify the addresses pointing to the files and 
2. Add ````~/```` in front of them:

{{<image src="/posts/images/devenv_2017-11-03_17-45-17.png">}}

If necessary, modern browser development tools can help identify missing files:

{{<image src="/posts/images/2017-11-03_17-58-39.png">}}

Once all references are corrected, we will have the page we already know, but this time generated by a Razor view from https://localhost:#####/CoreUI/Index.

#### 3.2 - Splitting the Index.cshtml view in components

Now let's split the Index.cshtml view in several components:

1. A "_Layout.cshtml" view
2. Several componentized partial views and 
3. the Index.cshtml view, with the main content of the page.

We're not going to show the whole process, just the final _Layout view and the resulting file list, so it should be pretty obvious what the job is and, ultimately, you can see the final result in the article's repository.

{{<renderSourceCode "html" "linenos=table">}}
<!--
* CoreUI - Open Source Bootstrap Admin Template
* @@version v1.0.4
* @@link http://coreui.io
* Copyright (c) 2017 creativeLabs Łukasz Holeczek
* @@license MIT
 -->
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name="description" content="CoreUI - Open Source Bootstrap Admin Template">
    <meta name="author" content="Łukasz Holeczek">
    <meta name="keyword" content="Bootstrap,Admin,Template,Open,Source,AngularJS,Angular,Angular2,Angular 2,Angular4,Angular 4,jQuery,CSS,HTML,RWD,Dashboard,React,React.js,Vue,Vue.js">
    <link rel="shortcut icon" href="img/favicon.png">

    <title>CoreUI - Open Source Bootstrap Admin Template</title>

    <!-- Icons -->
    <link href="~/vendors/css/font-awesome.min.css" rel="stylesheet">
    <link href="~/vendors/css/simple-line-icons.min.css" rel="stylesheet">

    <!-- Main styles for this application -->
    <link rel="stylesheet" href="~/css/style.css">

    <!-- Styles required by this view -->

</head>
<!-- BODY options, add following classes to body to change options

// Header options
1. '.header-fixed'                  - Fixed Header

// Brand options
1. '.brand-minimized'       - Minimized brand (Only symbol)

// Sidebar options
1. '.sidebar-fixed'                 - Fixed Sidebar
2. '.sidebar-hidden'                - Hidden Sidebar
3. '.sidebar-off-canvas'        - Off Canvas Sidebar
4. '.sidebar-minimized'         - Minimized Sidebar (Only icons)
5. '.sidebar-compact'             - Compact Sidebar

// Aside options
1. '.aside-menu-fixed'          - Fixed Aside Menu
2. '.aside-menu-hidden'         - Hidden Aside Menu
3. '.aside-menu-off-canvas' - Off Canvas Aside Menu

// Breadcrumb options
1. '.breadcrumb-fixed'          - Fixed Breadcrumb

// Footer options
1. '.footer-fixed'                  - Fixed footer

-->
<body class="app header-fixed sidebar-fixed aside-menu-fixed aside-menu-hidden">

    <!-- *APP-HEADER* -->
    @Html.Partial("_AppHeader")
    <!-- /*APP-HEADER* -->

    <div class="app-body">
        <div class="sidebar">

            <!-- *SIDEBAR-NAV* -->
            @Html.Partial("_SidebarNav")
            <!-- /*SIDEBAR-NAV* -->

            <button class="sidebar-minimizer brand-minimizer" type="button"></button>
        </div>

        <!-- *MAIN CONTENT* -->
        <main class="main">

            <!-- *BREADCRUMB* -->
            @Html.Partial("_BreadCrumb")
            <!-- /*BREADCRUMB* -->

            <!-- *CONTAINER-FLUID* -->
            <div class="container-fluid">

                <!-- *PAGE* -->
                @RenderBody()
                <!-- /*PAGE* -->

            </div>
            <!-- /*CONTAINER-FLUID* -->

        </main>
        <!-- /*MAIN CONTENT* -->

        <!-- *ASIDE-MENU* -->
        @Html.Partial("_AsideMenu")
        <!-- /*ASIDE-MENU* -->

    </div>
    <footer class="app-footer">
        <span><a href="http://coreui.io">CoreUI</a> © 2017 creativeLabs.</span>
        <span class="ml-auto">Powered by <a href="http://coreui.io">CoreUI</a></span>
    </footer>
    <!-- Bootstrap and necessary plugins -->
    <script src="~/vendors/js/jquery.min.js"></script>
    <script src="~/vendors/js/popper.min.js"></script>
    <script src="~/vendors/js/bootstrap.min.js"></script>
    <script src="~/vendors/js/pace.min.js"></script>
    <!-- Plugins and scripts required by all views -->
    <script src="~/vendors/js/Chart.min.js"></script>
    <!-- CoreUI main scripts -->
    <script src="~/js/app.js"></script>
    <!-- Plugins and scripts required by this views -->
    <!-- Custom scripts required by this view -->

</body>
</html>
{{</renderSourceCode>}}

{{<image src="/posts/images/devenv_2017-11-03_18-36-32.png">}}

If we now go back to https://localhost:#####/CoreUI/Index, we'll see the same screen, but this time as a composition of the main content on the layout and the partial views.

#### 3.3 - Convert the rest of the CoreUI pages

The conversion of the rest of the pages into razor views is quite simple, although there are some details that's better to look at directly in the repository.

The work is basically:

1. Leave just the content inside the `<div class="container-fluid">` tag, since this and everything else is, directly or indirectly (other componentized partial views), in **_Layout.cshtml** and

2. Change the extension to .cshtml

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
