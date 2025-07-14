using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using DevCommon.GUI;
using System.Collections.Generic;

public class CanvasView : CanvasBase
{
    private List<Selectable> allSelectables = new List<Selectable>();
    private List<SelectableData> selectableDatas = new List<SelectableData>();

    // On Reset canvas
    protected override sealed void OnReset() => resetCanvasComponents();

    // On Initialize canvas
    public override sealed void OnInitialize()
    {
        getAllSelectables();
        subscribedToEvents();
    }

    // On DeInitialize canvas
    public override sealed void OnDeInitialize() => unsubscribedFromEvents();

    // Reset Canvas components
    private void resetCanvasComponents()
    {
        if (GetComponent<CanvasScaler>())
        {
            DestroyImmediate(GetComponent<CanvasScaler>());
        }
    }

    // Get all selectables, which are not ignored by gui framework
    private void getAllSelectables()
    {
        allSelectables.Clear();
        selectableDatas.Clear();

        allSelectables = gameObject.GetComponentsInChildren<Selectable>(true).ToList();
        allSelectables.RemoveAll(x => x.GetComponent<GuiElementProperty>() != null && x.GetComponent<GuiElementProperty>().Ignored);
        allSelectables.ForEach(x => selectableDatas.Add(new SelectableData(x.name, x.GetType(), x)));
    }

    // Subscribe to ui events
    private void subscribedToEvents()
    {
        // Subscribe from Unity Ui Elemnent events
        getSelectables<Button>(allSelectables).ForEach(x => x.onClick.AddListener(() => OnSelect(x.name)));
        getSelectables<Toggle>(allSelectables).ForEach(x => x.onValueChanged.AddListener(delegate { OnUpdate(x.name, x.isOn); }));
        getSelectables<Dropdown>(allSelectables).ForEach(x => x.onValueChanged.AddListener(delegate { OnUpdate(x.name, x.options[x.value].text); }));
        getSelectables<Slider>(allSelectables).ForEach(x => x.onValueChanged.AddListener(delegate { OnUpdate(x.name, x.value); }));
        getSelectables<InputField>(allSelectables).ForEach(x =>
        {
            x.onValueChanged.AddListener(delegate { OnUpdate(x.name, x.text); });
            x.onEndEdit.AddListener(delegate { OnUpdate(x.name, x.text); });
        });

        // Subscribe to TMPro events
        getSelectables<TMP_Dropdown>(allSelectables).ForEach(x => x.onValueChanged.AddListener(delegate { OnUpdate(x.name, x.options[x.value].text); }));
        getSelectables<TMP_InputField>(allSelectables).ForEach(x =>
        {
            x.onValueChanged.AddListener(delegate { OnUpdate(x.name, x.text); });
            x.onEndEdit.AddListener(delegate { OnUpdate(x.name, x.text); });
        });
    }

    // Unsubscribe from ui events
    private void unsubscribedFromEvents()
    {
        // Unsubscribe from Unity Ui Elemnent events
        getSelectables<Button>(allSelectables).ForEach(x => x.onClick.RemoveAllListeners());
        getSelectables<Toggle>(allSelectables).ForEach(x => x.onValueChanged.RemoveAllListeners());
        getSelectables<Dropdown>(allSelectables).ForEach(x => x.onValueChanged.RemoveAllListeners());
        getSelectables<Slider>(allSelectables).ForEach(x => x.onValueChanged.RemoveAllListeners());
        getSelectables<InputField>(allSelectables).ForEach(x =>
        {
            x.onValueChanged.RemoveAllListeners();
            x.onEndEdit.RemoveAllListeners();
        });

        // Unsubscribe from TMPro events
        getSelectables<TMP_Dropdown>(allSelectables).ForEach(x => x.onValueChanged.RemoveAllListeners());
        getSelectables<TMP_InputField>(allSelectables).ForEach(x =>
        {
            x.onValueChanged.RemoveAllListeners();
            x.onEndEdit.RemoveAllListeners();
        });
    }

    // Get all selectables of type T
    private List<T> getSelectables<T>(List<Selectable> a_AllSelectables) where T : Selectable
    {
        List<T> t_AllSelectables = new List<T>();
        List<Selectable> t_TempSelectables = new List<Selectable>(a_AllSelectables);
        t_TempSelectables.RemoveAll(x => !Type.Equals(x.GetType(), typeof(T)));
        t_TempSelectables.ForEach(x => t_AllSelectables.Add((T)x));
        return t_AllSelectables;
    }

    // Check for Userinputs on late update
    private void LateUpdate()
    {
        checkUserButtonInput();
    }

    // Check if user has pressed any hardware button
    private void checkUserButtonInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackButton();
        }
        else if (Input.anyKeyDown)
        {
            OnKeyPressed();
        }
    }

    InputField inputField;
    TMP_InputField tmpInputField;
    Toggle toggle;
    Dropdown dropdown;
    // Get data from Selecatble item based on name
    protected T GetItemData<T>(string a_ItemName)
    {
        T t_ResultObj = default;
        SelectableData t_SelectableData = selectableDatas.Where(x => string.Equals(x.ObjName, a_ItemName)).FirstOrDefault();
        if (t_SelectableData != null)
        {
            if (Type.Equals(t_SelectableData.ObjType, typeof(InputField)))
            {
                inputField = (InputField)t_SelectableData.ObjItem;
                t_ResultObj = (T)Convert.ChangeType(inputField.text, typeof(T));
            }
            else if (Type.Equals(t_SelectableData.ObjType, typeof(TMP_InputField)))
            {
                tmpInputField = (TMP_InputField)t_SelectableData.ObjItem;
                t_ResultObj = (T)Convert.ChangeType(tmpInputField.text, typeof(T));
            }
            else if (Type.Equals(t_SelectableData.ObjType, typeof(Toggle)))
            {
                toggle = (Toggle)t_SelectableData.ObjItem;
                t_ResultObj = (T)Convert.ChangeType(toggle.isOn, typeof(T));
            }
            else if (Type.Equals(t_SelectableData.ObjType, typeof(Dropdown)))
            {
                dropdown = (Dropdown)t_SelectableData.ObjItem;
                t_ResultObj = Type.Equals(typeof(T), typeof(string)) ?
                    (T)Convert.ChangeType(dropdown.itemText, typeof(T)) : Type.Equals(typeof(T), typeof(Image)) ?
                    (T)Convert.ChangeType(dropdown.itemImage, typeof(T)) : Type.Equals(typeof(T), typeof(int)) ?
                    (T)Convert.ChangeType(dropdown.value, typeof(T)) : default(T);
            }
        }
        return t_ResultObj;
    }

    // Set data for Selecatble item based on name
    protected void SetItemData(string a_ItemName, object a_ItemData)
    {
        SelectableData t_SelectableData = selectableDatas.Where(x => string.Equals(x.ObjName, a_ItemName)).FirstOrDefault();
        if (t_SelectableData != null)
        {
            if (Type.Equals(t_SelectableData.ObjType, typeof(InputField)))
            {
                inputField = (InputField)t_SelectableData.ObjItem;
                inputField.text = (string)a_ItemData;
            }
            else if (Type.Equals(t_SelectableData.ObjType, typeof(TMP_InputField)))
            {
                tmpInputField = (TMP_InputField)t_SelectableData.ObjItem;
                tmpInputField.text = (string)a_ItemData;
            }
            else if (Type.Equals(t_SelectableData.ObjType, typeof(Toggle)))
            {
                toggle = (Toggle)t_SelectableData.ObjItem;
                toggle.isOn = (bool)a_ItemData;
            }
            else if (Type.Equals(t_SelectableData.ObjType, typeof(Dropdown)))
            {
                dropdown = (Dropdown)t_SelectableData.ObjItem;
                dropdown.value = (int)a_ItemData;
            }
        }
    }

    #region UiElement Event Methods
    // Called by default when a UiElement is selected
    protected virtual void OnSelect(string a_ItemName) {}

    // Called by default when a UiElement is updated or deselected
    protected virtual void OnUpdate(string a_ItemName, object a_ItemData) {}
    #endregion

    #region Hardware Button Event Methods
    // Called if any key has been pressed except for the escape key
    protected virtual void OnKeyPressed() {}

    // Called if escape key has been pressed
    protected virtual void OnBackButton() {}
    #endregion

    private sealed class SelectableData
    {
        public string ObjName { get; set; }
        public Type ObjType { get; set; }
        public Selectable ObjItem { get; set; }

        public SelectableData() { }

        public SelectableData(string a_Name, Type a_Type, Selectable a_Item)
        {
            ObjName = a_Name;
            ObjType = a_Type;
            ObjItem = a_Item;
        }
    }
}