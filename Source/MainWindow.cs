﻿/**
 * MainWindow.cs
 *
 * Thunder Aerospace Corporation's Part Lister for the Kerbal Space Program, by Taranis Elsu
 *
 * (C) Copyright 2013, Taranis Elsu
 *
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 *
 * This code is licensed under the Attribution-NonCommercial-ShareAlike 3.0 (CC BY-NC-SA 3.0)
 * creative commons license. See <http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode>
 * for full details.
 *
 * Attribution — You are free to modify this code, so long as you mention that the resulting
 * work is based upon or adapted from this code.
 *
 * Non-commercial - You may not use this work for commercial purposes.
 *
 * Share Alike — If you alter, transform, or build upon this work, you may distribute the
 * resulting work only under the same or similar license to the CC BY-NC-SA 3.0 license.
 *
 * Note that Thunder Aerospace Corporation is a ficticious entity created for entertainment
 * purposes. It is in no way meant to represent a real entity. Any similarity to a real entity
 * is purely coincidental.
 */

using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    class MainWindow : Window<TacPartLister>
    {
        private readonly string version;
        private readonly HashSet<Part> selectedParts = new HashSet<Part>();
        private Vector2 scrollPosition = Vector2.zero;

        private GUIStyle labelStyle;
        private GUIStyle labelStyle2;
        private GUIStyle headerStyle;
        private GUIStyle buttonStyle;
        private GUIStyle versionStyle;

        public MainWindow()
            : base("TAC Part Lister", 360, Screen.height * 0.6f)
        {
            this.Log("Constructor");
            version = Utilities.GetDllVersion(this);
        }

        protected override void DrawWindowContents(int windowId)
        {
            double totalMass = 0.0;
            double totalCost = 0.0;

            var parts = new List<Part>(EditorLogic.fetch.ship.parts);
            parts.ForEach(part => part.UpdateOrgPosAndRot(part.localRoot));
            parts.Sort(SortParts);

            GUILayout.BeginVertical();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Part Name", headerStyle);
            foreach (Part part in parts)
            {
                string partName = part.partInfo.title;
                if (partName.Length > 28)
                {
                    partName = partName.Substring(0, 28);
                }

                bool selected = GUILayout.Toggle(selectedParts.Contains(part), partName, buttonStyle);
                if (selected)
                {
                    selectedParts.Add(part);
                    part.SetHighlightColor(Color.blue);
                    part.SetHighlight(true);
                }
                else if (selectedParts.Remove(part))
                {
                    part.SetHighlightDefault();
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Stage", headerStyle);
            foreach (Part part in parts)
            {
                GUILayout.Label(part.inverseStage.ToString(), labelStyle2);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Mass", headerStyle);
            foreach (Part part in parts)
            {
                var mass = part.mass + part.GetResourceMass();
                GUILayout.Label(mass.ToString("#,##0.###"), labelStyle2);
                totalMass += mass;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Cost", headerStyle);
            foreach (Part part in parts)
            {
                GUILayout.Label(part.partInfo.cost.ToString("#,##0.#"), labelStyle2);
                totalCost += part.partInfo.cost;
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Label("Resources: ", headerStyle);
            foreach (KeyValuePair<string, ResourceInfo> entry in GetResources(parts))
            {
                GUILayout.Label("  " + entry.Key + " \t" + Utilities.FormatValue(entry.Value.amount) + "U \t" + Utilities.FormatValue(entry.Value.cost) + "K", labelStyle);
            }

            GUILayout.EndScrollView();

            GUILayout.Label("Parts: " + parts.Count.ToString() + ", Mass: " + totalMass.ToString("#,##0.######") + ", Cost: " + totalCost.ToString("#,##0.##"), labelStyle);

            GUILayout.EndVertical();

            GUI.Label(new Rect(4, windowPos.height - 13, windowPos.width - 20, 12), "TAC Part Lister v" + version, versionStyle);
        }

        protected override void ConfigureStyles()
        {
            base.ConfigureStyles();

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.wordWrap = false;
                labelStyle.fontStyle = FontStyle.Normal;
                labelStyle.normal.textColor = Color.white;
                labelStyle.alignment = TextAnchor.MiddleLeft;

                labelStyle2 = new GUIStyle(GUI.skin.label);
                labelStyle2.wordWrap = false;
                labelStyle2.fontStyle = FontStyle.Normal;
                labelStyle2.normal.textColor = Color.white;
                labelStyle2.alignment = TextAnchor.MiddleCenter;

                headerStyle = new GUIStyle(GUI.skin.label);
                headerStyle.wordWrap = false;
                headerStyle.fontStyle = FontStyle.Bold;
                headerStyle.normal.textColor = Color.white;
                headerStyle.alignment = TextAnchor.MiddleCenter;

                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.wordWrap = false;
                buttonStyle.fontStyle = FontStyle.Normal;
                buttonStyle.normal.textColor = Color.white;
                buttonStyle.alignment = TextAnchor.MiddleLeft;
                buttonStyle.padding = new RectOffset(6, 2, 4, 2);

                versionStyle = Utilities.GetVersionStyle();
            }
        }

        private static int SortParts(Part p1, Part p2)
        {
            return -p1.orgPos.y.CompareTo(p2.orgPos.y);
        }

        internal struct ResourceInfo
        {
            internal double amount;
            internal double cost;
        }

        private Dictionary<string, ResourceInfo> GetResources(List<Part> parts)
        {
            Dictionary<string, ResourceInfo> resourceInfos = new Dictionary<string, ResourceInfo>();

            foreach (Part p in parts)
            {
                foreach (PartResource r in p.Resources)
                {
                    ResourceInfo resourceInfo;
                    if (!resourceInfos.TryGetValue(r.resourceName, out resourceInfo))
                    {
                        resourceInfo = new ResourceInfo();
                    }

                    resourceInfo.amount += r.amount;
                    resourceInfo.cost += r.amount * r.info.unitCost;

                    resourceInfos[r.resourceName] = resourceInfo;
                }
            }

            return resourceInfos;
        }
    }
}
