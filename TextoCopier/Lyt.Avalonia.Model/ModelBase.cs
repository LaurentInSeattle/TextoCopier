﻿namespace Lyt.Avalonia.Model;

[AttributeUsage(AttributeTargets.Property)]
public class DoNotLogAttribute : Attribute { }

public abstract class ModelBase : IModel
{
    public ModelBase(IMessenger messenger, ILogger logger)
    {
        this.Messenger = messenger;
        this.Logger = logger;
        this.properties = []; 
    }

    public abstract Task Initialize();

    public virtual Task Configure(object? modelConfiguration) => Task.CompletedTask;

    public virtual Task Shutdown()
    {
        this.Clear();
        return Task.CompletedTask;
    }

    public virtual Task Save()
    {
        this.IsDirty = false ;
        return Task.CompletedTask;
    }

    public ILogger Logger { get; private set; }

    public IMessenger Messenger { get; private set; }

    public bool IsDirty { get; protected set; }

    /// <summary> Allows to disable logging when properties are changing so that we do not flood the logs. </summary>
    /// <remarks> Use for quickly changing properties, mouse, sliders, etc.</remarks>
    public bool DisablePropertyChangedLogging { get; protected set; }

    /// <summary> The model properties.</summary>
    protected readonly Dictionary<string, object?> properties;

    public void SubscribeToUpdates(Action<ModelUpdateMessage> onUpdate, bool withUiDispatch = false)
        => this.Messenger.Subscribe(onUpdate, withUiDispatch);

    protected void NotifyUpdate(string propertyName = "", string methodName="") 
        => this.Messenger.Publish(new ModelUpdateMessage(this, propertyName, methodName));

    /// <summary> Gets the value of a property </summary>
    protected T? Get<T>([CallerMemberName] string? name = null)
    {
        if (name is null)
        {
            this.Logger.Error("Get property: no name");
            throw new Exception("Get property: no name");
        }

        return this.properties.TryGetValue(name, out object? value) ? value == null ? default : (T)value : default;
    }

    /// <summary> Sets the value of a property </summary>
    /// <returns> True, if the value was changed, false otherwise. </returns>
    protected bool Set<T>(T? value, [CallerMemberName] string? name = null)
    {
        if (name is null)
        {
            this.Logger.Error("Set property: no name");
            throw new Exception("Set property: no name");
        }

        if (Equals(value, this.Get<T>(name)))
        {
            return false;
        }

        this.properties[name] = value;
        this.NotifyUpdate(name);
        this.IsDirty = true;
        if (!this.DisablePropertyChangedLogging)
        {
            this.LogPropertyChanged(name, value);
        }

        return true;
    }

    /// <summary> Clear and Dispose when applicable, all properties </summary>
    protected void Clear()
    {
        foreach (object? property in this.properties.Values)
        {
            if (property is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        this.properties.Clear();
    }

    #region Debug Utilities 

    /// <summary> Logs that a model property is changing. </summary>
    [Conditional("DEBUG")]
    private void LogPropertyChanged(string name, object? value)
    {
        int frameIndex = 1;
        string typeName;
        do
        {
            ++frameIndex;
            var frame = new StackFrame(frameIndex);
            var frameMethod = frame.GetMethod();
            if (frameMethod == null)
            {
                return;
            }

            typeName = frameMethod.DeclaringType!.Name;
        }
        while (typeName.StartsWith("ModelBase"));

        ++frameIndex;
        var frameAbove = new StackFrame(frameIndex);
        var methodAbove = frameAbove.GetMethod();
        if (methodAbove is not null)
        {
            var logAttribute = methodAbove.GetCustomAttribute<DoNotLogAttribute>();
            if (logAttribute is not null)
            {
                return;
            }
        }

        string methodAboveName = methodAbove != null ? methodAbove.Name : "<none>";
        string message =
            string.Format(
                "From {0} in {1}: Property {2} changed to:   {3}",
                typeName, methodAboveName, name, value == null ? "null" : value.ToString());
        this.Logger.Info(message);
    }

    #endregion Debug Utilities 
}