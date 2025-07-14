using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevCommon.GUI
{
    public sealed class CanvasRoot
    {
        private static CanvasRoot instance;
        private List<CanvasController> uiCanvasHandlers = new List<CanvasController>();

        // Private constructor
        private CanvasRoot() { }

        // Creating singleton
        public static CanvasRoot Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CanvasRoot();
                }
                return instance;
            }
        }

        // Register a controller
        public void Register(CanvasController a_Controller)
        {
            if (!uiCanvasHandlers.Contains(a_Controller))
            {
                uiCanvasHandlers.Add(a_Controller);
            }
        }

        // Unregister a controller by object
        public void Unregister(CanvasController a_Controller)
        {
            if (!uiCanvasHandlers.Contains(a_Controller))
            {
                uiCanvasHandlers.Remove(a_Controller);
            }
        }

        // Unregister a controller by name
        public void Unregister(string a_ControllerName)
        {
            CanvasController controller = GetController(a_ControllerName);
            if (controller != null)
            {
                uiCanvasHandlers.Remove(controller);
            }
        }

        // Get a controller by object
        public CanvasController GetController(string a_ControllerName)
        {
            CanvasController controller = uiCanvasHandlers.Where(x => x.ControllerName == a_ControllerName).FirstOrDefault();
            return controller;
        }

        // Get a controller by object
        public CanvasController GetControllerOrDefault(string a_ControllerName)
        {
            CanvasController controller = uiCanvasHandlers.Where(x => x.ControllerName == a_ControllerName).FirstOrDefault();
            if (controller == null)
            {
                controller = uiCanvasHandlers.FirstOrDefault();
            }
            return controller;
        }
    }
}
