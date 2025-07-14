using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DevCommon.Extended;
using DevCommon.Utils;

namespace DevCommon.GUI
{
    public sealed class CanvasController : Singleton<CanvasController>
    {
        [SerializeField]
        private Canvas parentCanvas;
        [SerializeField]
        private Camera renderingCamera;
        [SerializeField]
        private float planeDistance = 100;

        [SerializeField]
        private string controllerName = "UiMain";
        [SerializeField]
        private string defaultCanvas = "DefaultCanvas";

        [SerializeField]
        private Object resourcesDirectory;
        [SerializeField]
        private Object assemblyDirectory;

        private List<CanvasBase> hierarchyCanvasObjects = new List<CanvasBase>();
        private List<GameObject> resourceCanvasObjects = new List<GameObject>();

        private List<CanvasBase> activeCanvas = new List<CanvasBase>();
        private CStack<CanvasBase> canvasStack = new CStack<CanvasBase>();

        public string ControllerName { get => controllerName; }
        public Canvas ParentCanvas { get => parentCanvas; }

        public Object ResourcesDirectory { get => resourcesDirectory; }
        public Object AssemblyDirectory { get => assemblyDirectory; }

        protected override void Awake()
        {
            base.Awake();

            // Default rendering camera is Main Camera
            if (renderingCamera == null)
            {
                renderingCamera = Camera.main;
            }
            controllerName = controllerName.LetterOnly();

            assignCanvasRefInHierarchy();
            assignCanvasRefInResources();
            removeDuplicateCanvas();
        }

        private void Start()
        {
            CanvasRoot.Instance.Register(this);
            setupParentCanvas();
            loadDefaultCanvas();
        }

        // Assign all child canvas reference in Hierarchy
        private void assignCanvasRefInHierarchy()
        {
            hierarchyCanvasObjects = GetComponentsInChildren<CanvasBase>().ToList();
            hierarchyCanvasObjects.ForEach(x => x.Controller = this);
        }

        // Assign all canvas gameobject reference in Resources/controllerName
        private void assignCanvasRefInResources()
        {
            resourceCanvasObjects.Clear();
            UnityEngine.Object[] t_ResourceObjects = Resources.LoadAll(controllerName, typeof(GameObject));
            foreach (GameObject e in t_ResourceObjects)
            {
                GameObject t_Go = (GameObject)e;
                if (t_Go != null && t_Go.GetComponent<CanvasBase>())
                {
                    resourceCanvasObjects.Add(t_Go);
                }
            }
        }

        // Remove duplicate canvas reference from allResourceCanvasObjects list
        private void removeDuplicateCanvas() => resourceCanvasObjects.RemoveAll(x => hierarchyCanvasObjects.Exists(y => string.Equals(y.gameObject.name, x.name)));

        // Setup primary canvas
        private void setupParentCanvas()
        {
            parentCanvas.worldCamera = renderingCamera;
            parentCanvas.planeDistance = planeDistance;
        }

        // Load default canvas at start
        private void loadDefaultCanvas()
        {
            defaultCanvas = defaultCanvas.NonWhitespace();
            if (!string.IsNullOrEmpty(defaultCanvas) || !string.IsNullOrWhiteSpace(defaultCanvas))
            {
                LoadScreen(defaultCanvas);
            }
        }

        // Load a canvas by object from all canvas list
        public CanvasBase LoadScreen(CanvasBase a_CanvasScr, bool a_Animate = true)
        {
            if (!activeCanvas.Contains(a_CanvasScr))
            {
                onLoadScreen(a_CanvasScr, a_Animate);
            }
            else
            {
                a_CanvasScr.PutOnTop();
            }
            return a_CanvasScr;
        }

        // Load a canvas by name from all canvas list 
        public CanvasBase LoadScreen(string a_CanvasName, bool a_Animate = true)
        {
            CanvasBase t_ViewScr = GetActiveCanvasByName(a_CanvasName);
            if (t_ViewScr == null)
            {
                t_ViewScr = GetAnyCanvasByName(a_CanvasName);
                onLoadScreen(t_ViewScr, a_Animate);
            }
            else
            {
                t_ViewScr.PutOnTop();
            }
            return t_ViewScr;
        }

        // Load a canvas by direct reference
        private void onLoadScreen(CanvasBase a_CanvasScr, bool a_Animate)
        {
            if (a_CanvasScr != null)
            {
                a_CanvasScr.OnInitialize();
                a_CanvasScr.ResetCanvasAnchor();
                a_CanvasScr.PutOnTop();
                activeCanvas.Add(a_CanvasScr);
                if (a_Animate)
                {
                    a_CanvasScr.BeginRewindAnimationState(() => a_CanvasScr.BeginEntryAnimation(() => a_CanvasScr.EnableCanvasInteraction()));
                }
                else
                {
                    a_CanvasScr.EnableCanvasInteraction();
                }
            }
            else
            {
                Debug.LogError("<color=#ff1500>Canvas not available!</color>");
            }
        }

        // Get any canvas by name
        public CanvasBase GetAnyCanvasByName(string a_CanvasName)
        {
            CanvasBase t_CanvasBase = hierarchyCanvasObjects.Where(x => string.Equals(x.name, a_CanvasName)).FirstOrDefault();
            if (t_CanvasBase == null)
            {
                GameObject t_TempGo = resourceCanvasObjects.Where(x => string.Equals(x.name, a_CanvasName)).FirstOrDefault();
                GameObject t_CanvasGo;
                if (t_TempGo != null)
                {
                    t_CanvasGo = Instantiate(t_TempGo, parentCanvas.transform);
                    t_CanvasGo.name = a_CanvasName;
                    t_CanvasBase = t_CanvasGo.GetComponent<CanvasBase>();
                    t_CanvasBase.Controller = this;
                }
            }

            return t_CanvasBase;
        }

        // Get an active canvas by name
        public CanvasBase GetActiveCanvasByName(string a_CanvasName) => activeCanvas.Where(x => string.Equals(x.name, a_CanvasName)).FirstOrDefault();

        // Disable a canvas by name
        public void DestroyScreen(string a_CanvasName, bool a_Animate = true)
        {
            CanvasBase t_ViewScr = GetActiveCanvasByName(a_CanvasName);
            if (t_ViewScr != null)
            {
                DestroyScreen(t_ViewScr, a_Animate);
            }
        }

        // Disable a canvas by object
        public void DestroyScreen(CanvasBase a_CanvasScr, bool a_Animate = true)
        {
            if (activeCanvas.Contains(a_CanvasScr))
            {
                clearFromInternalStack(a_CanvasScr);
                if (a_Animate)
                {
                    a_CanvasScr.BeginExitAnimation(() => destroyCanvas(a_CanvasScr));
                }
                else
                {
                    destroyCanvas(a_CanvasScr);
                }
            }
        }

        // Destroy or hide canvas object
        private void destroyCanvas(CanvasBase a_CanvasScr)
        {
            activeCanvas.Remove(a_CanvasScr);
            if (loadedFromResources(a_CanvasScr))
            {
                Destroy(a_CanvasScr.gameObject);
            }
            else
            {
                a_CanvasScr.OnDeInitialize();
                a_CanvasScr.DisableCanvasInteraction();
                a_CanvasScr.ResetSortingOrder();
            }
        }

        // Check if canvas loaded from Resources
        private bool loadedFromResources(CanvasBase a_CanvasScr) => resourceCanvasObjects.Any(x => string.Equals(x.name, a_CanvasScr.gameObject.name));

        // Remove canvas from internal stack
        private void clearFromInternalStack(CanvasBase a_CanvasScr)
        {
            List<CanvasBase> t_StackedItems = canvasStack.GetAllItems();
            CanvasBase t_CanvasToRemove = t_StackedItems.Where(x => object.ReferenceEquals(x, a_CanvasScr)).FirstOrDefault();

            if (t_CanvasToRemove != null)
            {
                t_CanvasToRemove.OnDeInitialize();
                t_CanvasToRemove.DisableCanvasInteraction();
                canvasStack.Remove(t_CanvasToRemove);
            }
        }

        // Push All the active canvas into a stack (Do this on pop up UI)
        public void PushCanvas(CanvasBase a_CanvasScr)
        {
            if (activeCanvas.Contains(a_CanvasScr))
            {
                a_CanvasScr.OnDeInitialize();
                a_CanvasScr.DisableCanvasInteraction();
                canvasStack.Push(a_CanvasScr);
            }
        }

        // Push All the active canvas into a stack (Do this on pop up UI)
        public int PushAllActiveCanvas()
        {
            int t_PushCount = activeCanvas.Count;
            for (int i = 0; i < activeCanvas.Count; i++)
            {
                activeCanvas[i].OnDeInitialize();
                activeCanvas[i].DisableCanvasInteraction();
                canvasStack.Push(activeCanvas[i]);
            }
            activeCanvas.Clear();
            return t_PushCount;
        }

        // Pop canvas from stack and make active (Do this on closing pop up UI)
        public void PopCanvas(int a_PopCount = 1)
        {
            if (a_PopCount >= canvasStack.Count)
            {
                a_PopCount = canvasStack.Count;
            }

            for (int i = 0; i < a_PopCount; i++)
            {
                CanvasBase t_ViewScr = canvasStack.Pop();
                t_ViewScr.OnInitialize();
                t_ViewScr.EnableCanvasInteraction();
                activeCanvas.Add(t_ViewScr);
            }
        }

        // Pop canvas from stack and make active (Do this on closing pop up UI)
        public void PopAllCanvas()
        {
            PopCanvas(int.MaxValue);
        }

        // Remove all canvas objects
        public void RemoveAllCanvas()
        {
            for (int i = 0; i < activeCanvas.Count; i++)
            {
                DestroyScreen(activeCanvas[i], false);
            }
            activeCanvas.Clear();
        }

        // Set rendering order of a canvas
        public void SetCanvaseOrderInLayer(string a_CanvasName, int a_Order)
        {
            CanvasBase t_RequiredCanvas = GetActiveCanvasByName(a_CanvasName);
            if (t_RequiredCanvas != null)
            {
                t_RequiredCanvas.SetSortingOrder(a_Order);
            }
        }

        private void OnDisable()
        {
            CanvasRoot.Instance.Unregister(this);
        }

#if UNITY_EDITOR
        // Refresh gui assembly
        public void RefreshGuiAssembly()
        {
            controllerName = controllerName.LetterOnly();
            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrWhiteSpace(controllerName))
            {
                Debug.LogError("<color=#ff1500>Controller Name can't be Null or Empty!</color>");
                return;
            }
            GuiAssemblyHelper.RefreshGuiAssembly(this);
        }
#endif
    }
}