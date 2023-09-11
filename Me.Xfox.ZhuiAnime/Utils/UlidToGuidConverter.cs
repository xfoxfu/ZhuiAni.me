using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Me.Xfox.ZhuiAnime.Utils;

public class UlidToGuidConverter : ValueConverter<Ulid, Guid>
{
    private static readonly ConverterMappingHints defaultHints = new(size: 16);

    public UlidToGuidConverter() : this(null)
    {
    }

    public UlidToGuidConverter(ConverterMappingHints? mappingHints = null)
        : base(
                convertToProviderExpression: x => new Guid(x.ToByteArray()),
                convertFromProviderExpression: x => new Ulid(x),
                mappingHints: defaultHints.With(mappingHints))
    {
    }
}
