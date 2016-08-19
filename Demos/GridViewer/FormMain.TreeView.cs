﻿using CSharpGL;
using SimLab.helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TracyEnergy.Simba.Data.Keywords.impl;

namespace GridViewer
{
    public partial class FormMain
    {

        private void objectsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node as AbstractTreeNode;
            if (node != null)
            {
                node.Selected(sender, e);
                this.scientificCanvas.Invalidate();
            }

            this.propertyGrid1.SelectedObject = e.Node.Tag;

            //if (e.Node.Tag is GridBlockProperty)
            //{
            //    var property = e.Node.Tag as GridBlockProperty;
            //    var sceneObject = e.Node.Parent.Tag as SceneObject;
            //    BoundedRenderer boundedRenderer = (sceneObject.Renderer as BoundedRendererComponent).Renderer;
            //    if (boundedRenderer.Renderer is CatesianGridRenderer)
            //    {
            //        CatesianGrid grid = (boundedRenderer.Renderer as CatesianGridRenderer).Grid;
            //        UpdateCatesianGrid(grid, property);
            //    }
            //    this.scientificCanvas.Invalidate();
            //}
            //else if (e.Node.ToolTipText == typeof(CatesianGrid).Name)
            //{
            //    var sceneObject = e.Node.Tag as SceneObject;
            //    CatesianGrid grid = ((sceneObject.Renderer as BoundedRendererComponent).Renderer.Renderer as CatesianGridRenderer).Grid;
            //    GridBlockProperty property = grid.GridBlockProperties[0];
            //    UpdateCatesianGrid(grid, property);
            //    this.scientificCanvas.Invalidate();
            //}
        }

        private void UpdateCatesianGrid(CatesianGrid grid, GridBlockProperty property)
        {
            double axisMin, axisMax, step;
            ColorIndicatorAxisAutomator.Automate(property.MinValue, property.MaxValue, out axisMin, out axisMax, out step);
            grid.MinColorCode = (float)axisMin;
            grid.MaxColorCode = (float)axisMax;
            grid.UpdateColor(property);
            this.scientificCanvas.ColorPalette.SetCodedColor(axisMin, axisMax, step);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.objectsTreeView.SelectedNode;
            if (node != null)
            {
                SceneObject obj = node.Tag as SceneObject;
                if (obj != null)
                {
                    if (obj.Parent == null)
                    {
                        this.scientificCanvas.Scene.ObjectList.Remove(obj);
                        this.objectsTreeView.Nodes.Remove(node);
                    }
                    else
                    {
                        obj.Parent.Children.Remove(obj);
                        node.Parent.Nodes.Remove(node);
                    }

                    obj.Dispose();
                }

                this.scientificCanvas.Invalidate();
            }
        }

        /// <summary>
        /// move camera to focus on selected scene object again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void adjustCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.objectsTreeView.SelectedNode;
            if (node != null)
            {
                SceneObject obj = node.Tag as SceneObject;
                if (obj != null)
                {
                    var renderer = obj.Renderer as GridViewRenderer;
                    vec3 position = renderer.ModelMatrix.GetTranslate();
                    GenerateBoxScript script = obj.GetScript<GenerateBoxScript>();
                    IBoundingBox box = script.BoxRenderer;
                    IBoundingBox translatedBox = new BoundingBox(box.MinPosition + position, box.MaxPosition + position);
                    translatedBox.ZoomCamera(this.scientificCanvas.Scene.Camera);
                }
                this.scientificCanvas.Invalidate();
            }
        }
    }
}
