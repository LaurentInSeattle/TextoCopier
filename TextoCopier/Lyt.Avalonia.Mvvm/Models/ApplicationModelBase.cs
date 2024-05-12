﻿namespace Lyt.Avalonia.Mvvm.Models;

public class ApplicationModelBase(ILogger logger, ApplicationBase application) : IApplicationModel
{
    private readonly ILogger logger = logger;
    private readonly ApplicationBase application = application;

    public async Task Initialize()
    {
        try
        {
            foreach (var model in this.application.GetModels())
            {
                await model.Initialize();
            }
        }
        catch (Exception ex)
        {
            // Should never fail here
            if (Debugger.IsAttached) { Debugger.Break(); }
            this.logger.Error(ex.ToString());
            throw new ApplicationException("Failed to initialize models.", ex);
        }

        try
        {
            // Launch cleanup threads 
            // FileHelpers.LaunchAllCleanupThreads();
            //await MiniProfiler.FullGcCollect();
            //MiniProfiler.MemorySnapshot("System software initialization complete");
        }
        catch (Exception ex)
        {
            // Should never fail here
            if (Debugger.IsAttached) { Debugger.Break(); }
            this.logger.Error(ex.ToString());
            throw new ApplicationException("Failed to cleanup on startup.", ex);
        }
    }

    public async Task Shutdown()
    {
        try
        {
            foreach (var model in this.application.GetModels())
            {
                await model.Shutdown();
            }
        }
        catch (Exception ex)
        {
            // Should never fail here
            if (Debugger.IsAttached) { Debugger.Break(); }
            this.logger.Error(ex.ToString());
            throw;
        }
    }
}
