---
language: en
title: How to do integration testing for a Bot Framework SDK v4 event bot?
draft: true
author: "Miguel Veloso"
date: 2019-07-05
description: Continue the exploration of Bot Framework SDK enabling our Web API bot to receive arbitrary events through a REST API endpoint.
thumbnail: posts/how-to-do-integration-testing-bot-framework-v4-event-bot/ildefonso-polo-DX9X0g0Cg88-unsplash.jpg
tags: [ "Bot Builder v4", "ASP.NET Core", "Testing" ]
repoName: GAB2019-EventBot
toc: true
image:
    caption: A telecommunication test set...
    authorName: Ildefonso Polo
    url: https://unsplash.com/photos/DX9X0g0Cg88
---

### Event tests

This is a quite interesting topic because we use ASP.NET Core Integration testing and Bot Framework testing in the same project, thanks to the fact that we are using a Web API project for driving the bot.

Most of the work here is based on the official [Integration tests in ASP.NET Core documentation page](https://docs.microsoft.com/aspnet/core/test/integration-tests), although we'll use a little different approach, needed to test using Theories.

#### The Program class

#### The IntegrationTestsStartup class

#### The BotControllerTests test class

##### Constructor

##### GetService method

##### SendAsync method

##### AssertReplyAsync methods

##### BotShouldEchoBack test method

##### BotShouldCreateTimerAndSendProactiveMessage test method

##### BotShouldSendMessage_WhenEventReceived test method
