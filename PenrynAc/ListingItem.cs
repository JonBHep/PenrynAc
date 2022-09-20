using System;

namespace PenrynAc;

public struct ListingItem : IEquatable<ListingItem>
{
    private readonly int _intValue;
    private readonly string _rubric;

    public ListingItem(int itemIndex, string itemRubric)
    {
        _intValue = itemIndex;
        _rubric = itemRubric;
    }

    public override string ToString()
    {
        return _rubric;
    }

    public int ItemIndex => _intValue;

    public override bool Equals(object? obj)
    {
        bool eq = true;
        if (obj is ListingItem itm)
        {
            if (!ItemIndex.Equals(itm.ItemIndex))
            {
                eq = false;
            }

            if (!ToString().Equals(itm.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                eq = false;
            }

            return eq;
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = (int) 2166136261;
            // Suitable nullity checks etc, of course :)
            hash = (hash * 16777619) ^ _intValue.GetHashCode();
            hash = (hash * 16777619) ^ _rubric.GetHashCode();
            return hash;
        }
    }

    public bool Equals(ListingItem other)
    {
        var eq = ItemIndex.Equals(other.ItemIndex);

        if (!ToString().Equals(other.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            eq = false;
        }

        return eq;
    }

    public static bool operator ==(ListingItem left, ListingItem right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ListingItem left, ListingItem right)
    {
        return !(left == right);
    }
    
}