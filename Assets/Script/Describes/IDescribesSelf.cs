using System.Collections.Generic;
using UnityEngine;

public interface IDescribesSelf
{
    IEnumerable<string> GetDescriptionLines(Unit target, IEnumerable<StatModifier> myMods);
}
