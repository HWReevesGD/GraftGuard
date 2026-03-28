using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting;
internal class Inventory
{
    // Inventory Dictionary
    private Dictionary<string, int> _parts = [];
    public List<TowerDesign> StartingDesigns = [];

    /// <summary>
    /// Returns the count for the given <paramref name="part"/>
    /// </summary>
    /// <param name="part">Part to get count for</param>
    /// <returns>Number of this part in inventory</returns>
    public int GetPartCount(PartDefinition part) => GetPartCount(part.Name);
    /// <summary>
    /// Returns the count for the given <paramref name="partName"/>
    /// </summary>
    /// <param name="partName">Part to get count for</param>
    /// <returns>Number of this part in inventory</returns>
    public int GetPartCount(string partName)
    {
        partName = partName.ToLower();
        return _parts.GetValueOrDefault(partName, 0);
    }
    /// <summary>
    /// Sets the count for the give <paramref name="part"/>
    /// </summary>
    /// <param name="part">Part to set count for</param>
    /// <param name="value">Count value to set</param>
    public void SetPartCount(PartDefinition part, int value) => SetPartCount(part.Name, value);
    /// <summary>
    /// Sets the count for the give <paramref name="partName"/>
    /// </summary>
    /// <param name="partName">Part to set count for</param>
    /// <param name="value">Count value to set</param>
    public void SetPartCount(string partName, int value)
    {
        partName = partName.ToLower();
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException("Cannot modify a part count to a negative number");
        }
        _parts[partName] = value;
    }
    /// <summary>
    /// Modifiers the count of the given <paramref name="part"/>
    /// </summary>
    /// <param name="part">Part to modify count of</param>
    /// <param name="change">Amount to modify count by</param>
    public void ModifyPartCount(PartDefinition part, int change) => ModifyPartCount(part.Name, change);
    /// <summary>
    /// Modifiers the count of the given <paramref name="partName"/>
    /// </summary>
    /// <param name="partName">Part to modify count of</param>
    /// <param name="change">Amount to modify count by</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the part count is changed below zero</exception>
    public void ModifyPartCount(string partName, int change)
    {
        partName = partName.ToLower();
        if (GetPartCount(partName) + change < 0)
        {
            throw new ArgumentOutOfRangeException("Cannot modify a part count to a negative number");
        }
        _parts[partName] = GetPartCount(partName) + change;
    }
}
