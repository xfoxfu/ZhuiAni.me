using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Me.Xfox.ZhuiAnime.Utils;

public abstract class ZAModelBinder<T> : IModelBinder
{
    protected abstract string ModelName { get; }
    protected abstract ValueTask<T?> GetValue(uint id);
    protected abstract ZhuiAnimeError GetError(uint id);

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;

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
        bindingContext.Result = ModelBindingResult.Success(model);
    }
}
