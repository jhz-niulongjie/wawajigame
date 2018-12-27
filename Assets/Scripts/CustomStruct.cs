using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CustomStruct<K, V>
{
    public K key;
    public V value;

    public CustomStruct(K _key,V _value)
    {
        key = _key;
        value = _value;
    }
}
