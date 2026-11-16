using System;
using System.Collections.Generic;
using UnityEngine;

public static class NoteProgress
{
    private static readonly Dictionary<string, HashSet<UnityEngine.Object>> allNotes =
        new Dictionary<string, HashSet<UnityEngine.Object>>();

    private static readonly Dictionary<string, HashSet<UnityEngine.Object>> collectedNotes =
        new Dictionary<string, HashSet<UnityEngine.Object>>();

    private static readonly Dictionary<UnityEngine.Object, string> zoneOfNote =
        new Dictionary<UnityEngine.Object, string>();

    public static string CurrentZone { get; private set; }

    public static event Action Changed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        allNotes.Clear();
        collectedNotes.Clear();
        zoneOfNote.Clear();
        CurrentZone = null;
        Changed = null;
    }

    public static void Register(UnityEngine.Object note, string zoneId)
    {
        if (note == null || string.IsNullOrEmpty(zoneId)) return;

        if (!allNotes.TryGetValue(zoneId, out var set))
        {
            set = new HashSet<UnityEngine.Object>();
            allNotes[zoneId] = set;
        }

        set.Add(note);
        zoneOfNote[note] = zoneId;
        Changed?.Invoke();
    }

    public static void Unregister(UnityEngine.Object note)
    {
        if (note == null || !zoneOfNote.TryGetValue(note, out string zoneId)) return;

        zoneOfNote.Remove(note);
        if (allNotes.TryGetValue(zoneId, out var all)) all.Remove(note);
        if (collectedNotes.TryGetValue(zoneId, out var got)) got.Remove(note);
        Changed?.Invoke();
    }

    /// <summary>Marca la nota como leída. Si ya estaba marcada, no cambia nada.</summary>
    public static void Collect(UnityEngine.Object note)
    {
        if (note == null || !zoneOfNote.TryGetValue(note, out string zoneId)) return;

        if (!collectedNotes.TryGetValue(zoneId, out var set))
        {
            set = new HashSet<UnityEngine.Object>();
            collectedNotes[zoneId] = set;
        }

        if (set.Add(note))
            Changed?.Invoke();
    }

    public static void SetCurrentZone(string zoneId)
    {
        if (CurrentZone == zoneId) return;
        CurrentZone = zoneId;
        Changed?.Invoke();
    }

    public static int GetTotal(string zoneId)
    {
        return zoneId != null && allNotes.TryGetValue(zoneId, out var set) ? set.Count : 0;
    }

    public static int GetCollected(string zoneId)
    {
        return zoneId != null && collectedNotes.TryGetValue(zoneId, out var set) ? set.Count : 0;
    }
}