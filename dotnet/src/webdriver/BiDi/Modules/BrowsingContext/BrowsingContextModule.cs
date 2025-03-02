// <copyright file="BrowsingContextModule.cs" company="Selenium Committers">
// Licensed to the Software Freedom Conservancy (SFC) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The SFC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.BiDi.Communication;

#nullable enable

namespace OpenQA.Selenium.BiDi.Modules.BrowsingContext;

public class BrowsingContextModule(Broker broker) : Module(broker)
{
    public async Task<BrowsingContext> CreateAsync(ContextType type, CreateOptions? options = null)
    {
        var @params = new CreateCommandParameters(type);

        if (options is not null)
        {
            @params.ReferenceContext = options.ReferenceContext;
            @params.Background = options.Background;
            @params.UserContext = options.UserContext;
        }

        var createResult = await Broker.ExecuteCommandAsync<CreateCommand, CreateResult>(new CreateCommand(@params), options).ConfigureAwait(false);

        return createResult.Context;
    }

    public async Task<NavigateResult> NavigateAsync(BrowsingContext context, string url, NavigateOptions? options = null)
    {
        var @params = new NavigateCommandParameters(context, url);

        if (options is not null)
        {
            @params.Wait = options.Wait;
        }

        return await Broker.ExecuteCommandAsync<NavigateCommand, NavigateResult>(new NavigateCommand(@params), options).ConfigureAwait(false);
    }

    public async Task ActivateAsync(BrowsingContext context, ActivateOptions? options = null)
    {
        var @params = new ActivateCommandParameters(context);

        await Broker.ExecuteCommandAsync(new ActivateCommand(@params), options).ConfigureAwait(false);
    }

    public async Task<LocateNodesResult> LocateNodesAsync(BrowsingContext context, Locator locator, LocateNodesOptions? options = null)
    {
        var @params = new LocateNodesCommandParameters(context, locator);

        if (options is not null)
        {
            @params.MaxNodeCount = options.MaxNodeCount;
            @params.SerializationOptions = options.SerializationOptions;
            @params.StartNodes = options.StartNodes;
        }

        return await Broker.ExecuteCommandAsync<LocateNodesCommand, LocateNodesResult>(new LocateNodesCommand(@params), options).ConfigureAwait(false);
    }

    public async Task<CaptureScreenshotResult> CaptureScreenshotAsync(BrowsingContext context, CaptureScreenshotOptions? options = null)
    {
        var @params = new CaptureScreenshotCommandParameters(context);

        if (options is not null)
        {
            @params.Origin = options.Origin;
            @params.Format = options.Format;
            @params.Clip = options.Clip;
        }

        return await Broker.ExecuteCommandAsync<CaptureScreenshotCommand, CaptureScreenshotResult>(new CaptureScreenshotCommand(@params), options).ConfigureAwait(false);
    }

    public async Task CloseAsync(BrowsingContext context, CloseOptions? options = null)
    {
        var @params = new CloseCommandParameters(context);

        await Broker.ExecuteCommandAsync(new CloseCommand(@params), options).ConfigureAwait(false);
    }

    public async Task<TraverseHistoryResult> TraverseHistoryAsync(BrowsingContext context, int delta, TraverseHistoryOptions? options = null)
    {
        var @params = new TraverseHistoryCommandParameters(context, delta);

        return await Broker.ExecuteCommandAsync<TraverseHistoryCommand, TraverseHistoryResult>(new TraverseHistoryCommand(@params), options).ConfigureAwait(false);
    }

    public async Task<NavigateResult> ReloadAsync(BrowsingContext context, ReloadOptions? options = null)
    {
        var @params = new ReloadCommandParameters(context);

        if (options is not null)
        {
            @params.IgnoreCache = options.IgnoreCache;
            @params.Wait = options.Wait;
        }

        return await Broker.ExecuteCommandAsync<ReloadCommand, NavigateResult>(new ReloadCommand(@params), options).ConfigureAwait(false);
    }

    public async Task SetViewportAsync(BrowsingContext context, SetViewportOptions? options = null)
    {
        var @params = new SetViewportCommandParameters(context);

        if (options is not null)
        {
            @params.Viewport = options.Viewport;
            @params.DevicePixelRatio = options?.DevicePixelRatio;
        }

        await Broker.ExecuteCommandAsync(new SetViewportCommand(@params), options).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<BrowsingContextInfo>> GetTreeAsync(GetTreeOptions? options = null)
    {
        var @params = new GetTreeCommandParameters();

        if (options is not null)
        {
            @params.MaxDepth = options.MaxDepth;
            @params.Root = options.Root;
        }

        var getTreeResult = await Broker.ExecuteCommandAsync<GetTreeCommand, GetTreeResult>(new GetTreeCommand(@params), options).ConfigureAwait(false);

        return getTreeResult.Contexts;
    }

    public async Task<PrintResult> PrintAsync(BrowsingContext context, PrintOptions? options = null)
    {
        var @params = new PrintCommandParameters(context);

        if (options is not null)
        {
            @params.Background = options.Background;
            @params.Margin = options.Margin;
            @params.Orientation = options.Orientation;
            @params.Page = options.Page;
            @params.PageRanges = options.PageRanges;
            @params.Scale = options.Scale;
            @params.ShrinkToFit = options.ShrinkToFit;
        }

        return await Broker.ExecuteCommandAsync<PrintCommand, PrintResult>(new PrintCommand(@params), options).ConfigureAwait(false);
    }

    public async Task HandleUserPromptAsync(BrowsingContext context, HandleUserPromptOptions? options = null)
    {
        var @params = new HandleUserPromptCommandParameters(context);

        if (options is not null)
        {
            @params.Accept = options.Accept;
            @params.UserText = options.UserText;
        }

        await Broker.ExecuteCommandAsync(new HandleUserPromptCommand(@params), options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnNavigationStartedAsync(Func<NavigationInfo, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.navigationStarted", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnNavigationStartedAsync(Action<NavigationInfo> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.navigationStarted", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnFragmentNavigatedAsync(Func<NavigationInfo, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.fragmentNavigated", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnFragmentNavigatedAsync(Action<NavigationInfo> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.fragmentNavigated", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnDomContentLoadedAsync(Func<NavigationInfo, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.domContentLoaded", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnDomContentLoadedAsync(Action<NavigationInfo> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.domContentLoaded", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnLoadAsync(Func<NavigationInfo, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.load", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnLoadAsync(Action<NavigationInfo> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.load", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnDownloadWillBeginAsync(Func<NavigationInfo, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.downloadWillBegin", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnDownloadWillBeginAsync(Action<NavigationInfo> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.downloadWillBegin", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnNavigationAbortedAsync(Func<NavigationInfo, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.navigationAborted", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnNavigationAbortedAsync(Action<NavigationInfo> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.navigationAborted", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnNavigationFailedAsync(Func<NavigationInfo, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.navigationFailed", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnNavigationFailedAsync(Action<NavigationInfo> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.navigationFailed", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnContextCreatedAsync(Func<BrowsingContextInfo, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.contextCreated", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnContextCreatedAsync(Action<BrowsingContextInfo> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.contextCreated", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnContextDestroyedAsync(Func<BrowsingContextInfo, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.contextDestroyed", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnContextDestroyedAsync(Action<BrowsingContextInfo> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.contextDestroyed", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnUserPromptOpenedAsync(Func<UserPromptOpenedEventArgs, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.userPromptOpened", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnUserPromptOpenedAsync(Action<UserPromptOpenedEventArgs> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.userPromptOpened", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnUserPromptClosedAsync(Func<UserPromptClosedEventArgs, Task> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.userPromptClosed", handler, options).ConfigureAwait(false);
    }

    public async Task<Subscription> OnUserPromptClosedAsync(Action<UserPromptClosedEventArgs> handler, BrowsingContextsSubscriptionOptions? options = null)
    {
        return await Broker.SubscribeAsync("browsingContext.userPromptClosed", handler, options).ConfigureAwait(false);
    }
}
