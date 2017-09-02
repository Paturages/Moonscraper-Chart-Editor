﻿using UnityEngine;
using System.Collections;
using System;

public class PlaceBPM : PlaceSongObject {
    public BPM bpm { get { return (BPM)songObject; } set { songObject = value; } }
    new public BPMController controller { get { return (BPMController)base.controller; } set { base.controller = value; } }
    protected bool setAsLastBpm = true;

    protected override void SetSongObjectAndController()
    {
        bpm = new BPM();

        controller = GetComponent<BPMController>();
        controller.bpm = bpm;
    }

    protected override void Controls()
    {
        if (!Globals.lockToStrikeline)
        {
            if (Toolpane.currentTool == Toolpane.Tools.BPM && Globals.applicationMode == Globals.ApplicationMode.Editor && Input.GetMouseButtonDown(0))
            {
                RecordAddActionHistory(bpm, editor.currentSong.bpms);

                AddObject();
            }
        }
        else if (Input.GetButtonDown("Add Object"))
        {
            SongObject[] searchArray = editor.currentSong.syncTrack;
            int pos = SongObject.FindObjectPosition(bpm, searchArray);
            if (pos == SongObject.NOTFOUND)
            {
                editor.actionHistory.Insert(new ActionHistory.Add(bpm));
                AddObject();
            }
            else if (searchArray[pos].position != 0)
            {
                editor.actionHistory.Insert(new ActionHistory.Delete(searchArray[pos]));
                searchArray[pos].Delete();
                editor.currentSelectedObject = null;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (setAsLastBpm)
        {
            // Set BPM value to the last bpm in the chart from the current position
            int lastBpmArrayPos = SongObject.FindClosestPosition(bpm.position, editor.currentSong.bpms);

            if (editor.currentSong.bpms[lastBpmArrayPos].position > bpm.position)
                --lastBpmArrayPos;

            if (lastBpmArrayPos != SongObject.NOTFOUND && lastBpmArrayPos >= 0)
            {
                bpm.value = editor.currentSong.bpms[lastBpmArrayPos].value;
            }
        }
    }

    protected override void AddObject()
    {
        AddObjectToCurrentSong(bpm, editor);
        /*
        BPM bpmToAdd = new BPM(bpm);
        editor.currentSong.Add(bpmToAdd);
        editor.CreateBPMObject(bpmToAdd);
        editor.currentSelectedObject = bpmToAdd;*/
    }

    public static void AddObjectToCurrentSong(BPM bpm, ChartEditor editor, bool update = true)
    {
        BPM bpmToAdd = new BPM(bpm);
        editor.currentSong.Add(bpmToAdd, update);
        //editor.CreateBPMObject(bpmToAdd);
        editor.currentSelectedObject = bpmToAdd;
    }
}
