using UnityEngine;
using UnityEngine.UI;
using System;

namespace DevCommon.GUI
{
    [DisallowMultipleComponent]
    public class Cell<T> : MonoBehaviour
    {
        [SerializeField] protected Button customButton;

        protected T cellData;
        protected Action<T> onClickCallback;

        private void Awake()
        {
            if (customButton == null)
            {
                Button t_Button = GetComponent<Button>();
                if (t_Button != null)
                    t_Button.onClick.AddListener(onClick);
            }
            else
                customButton.onClick.AddListener(onClick);
        }

        protected virtual void onClick()
        {
            onClickCallback?.Invoke(cellData);
        }

        public virtual void InitializeCell(T a_CellData, Action<T> a_OnClickCallback = null)
        {
            cellData = a_CellData;
            onClickCallback = a_OnClickCallback;
        }

        public virtual void InitializeCallback(Action<T> a_OnClickCallback = null)
        {
            onClickCallback = a_OnClickCallback;
        }

        public virtual T CellData()
        {
            return cellData;
        }
    }
}
