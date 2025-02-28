using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace CherryUI.InteractiveElements.Populators
{
    public abstract class PopulatorBase<T>
        where T : class
    {
        protected IEnumerable<T> Data;
        protected PopulatorElementBase<T> ElementSample;
        protected Transform ElementsRoot;

        protected List<PopulatorElementBase<T>> elements = new();

        protected PopulatorBase(PopulatorElementBase<T> elementSample, Transform root)
        {
            ElementSample = elementSample;
            ElementsRoot = root;
        }

        public virtual void UpdateElements(IEnumerable<T> data, float delayEveryElement = 0f)
        {
            Data = data;

            var seq = DOTween.Sequence();

            var enumerableData = Data as T[] ?? Data?.ToArray() ?? System.Array.Empty<T>();
            var index = 0;
            var currentDelay = 0f;

            for (; index < enumerableData.Length; index++)
            {
                currentDelay = index * delayEveryElement;
                PopulatorElementBase<T> currentElement;

                if (index < elements.Count)
                {
                    currentElement = elements[index];
                    if (currentElement.isActiveAndEnabled)
                    {
                        currentElement.SetData(enumerableData[index]);
                        seq.Insert(currentDelay, currentElement.Refresh());
                    }
                    else
                    {
                        currentElement.gameObject.SetActive(true);
                        currentElement.SetData(enumerableData[index]);
                        seq.Insert(currentDelay, currentElement.Show());
                    }
                }
                else
                {
                    currentElement = Object.Instantiate(ElementSample, ElementsRoot);
                    currentElement.gameObject.SetActive(true);
                    currentElement.SetData(enumerableData[index]);
                    seq.Insert(currentDelay, currentElement.Show());
                    elements.Add(currentElement);
                }
            }

            for (; index < elements.Count; index++)
            {
                var currentElement = elements[index];
                currentElement.SetData(null);
                if (currentElement.isActiveAndEnabled)
                {
                    currentDelay = index * delayEveryElement;
                    seq.Insert(currentDelay, currentElement.Hide());
                    seq.AppendCallback(() => currentElement.gameObject.SetActive(false));
                }
            }
        }
    }
}