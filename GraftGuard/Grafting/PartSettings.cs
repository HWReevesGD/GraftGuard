using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraftGuard.Grafting.Registry.Behaviors;

namespace GraftGuard.Grafting;
/// <summary>
/// Custom per-tower or per-enemy "Settings" which are passed to part behaviors. This exists to not make the already bloated <see cref="IPartBehavior"/> methods more bloated.
/// Feel free to add any setting you need to pass to a part behavior here. Use this syntax when instantiating:
/// <code>
/// new TowerSettings() {
///     VariableName = Value,
///     etc = etc,
///     etc...
/// }
/// </code>
/// </summary>
internal struct PartSettings
{
    public PartSettings() {}
    public static PartSettings Default => new PartSettings();
    public bool PartsAreVertical = false;
}
