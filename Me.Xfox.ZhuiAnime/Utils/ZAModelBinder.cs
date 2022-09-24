using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;

namespace Me.Xfox.ZhuiAnime.Utils;

public abstract class ZAModelBinder<T> : IModelBinder
{
    protected readonly ILogger Logger;

    protected ZAModelBinder(ILogger logger)
    {
        Logger = logger;
    }

    protected abstract string ModelName { get; }
    protected abstract Task<T?> GetValue(uint id);
    protected abstract ZhuiAnimeError GetError(uint id);

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;
        Logger.Information("Finding binding for {ModelName}", modelName);

        // Try to fetch the value of the argument by name
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None) return;

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        // Check if the argument value is null or empty
        if (string.IsNullOrEmpty(value)) return;

        if (!uint.TryParse(value, out var id))
        {
            bindingContext.ModelState.TryAddModelError(
                modelName, $"{ModelName} must be an integer.");

            return;
        }

        var model = await GetValue(id);
        if (model == null) throw GetError(id);

        Logger.Information("Adding {ItemKey} to HttpContext.Items", typeof(T));
        bindingContext.HttpContext.Items.Add(typeof(T), model);

        bindingContext.Result = ModelBindingResult.Success(model);
    }
}
