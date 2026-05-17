#pragma warning disable CS0618 // Type or member is obsolete
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NoOpArmy.WiseFeline
{
    public class DragAndDropManipulator : PointerManipulator
    {
        public DragAndDropManipulator(VisualElement target, FieldView fieldView)
        {
            this.target = target;
            this.fieldView = fieldView;

            VisualElement currentElement = fieldView;
            while (currentElement != null)
            {
                scrollView = currentElement as ScrollView;
                if (scrollView != null)
                {
                    break;
                }

                // Move up to the parent element
                currentElement = currentElement.parent;
            }
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler, TrickleDown.TrickleDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        private Vector2 targetStartPosition { get; set; }

        private Vector3 pointerStartPosition { get; set; }

        private bool enabled { get; set; }

        private FieldView fieldView;
        private VisualElement socket;
        private VisualElement list;
        ScrollView scrollView = null;

        private void PointerDownHandler(PointerDownEvent evt)
        {
            socket = fieldView.parent;
            list = socket.parent;

            targetStartPosition = fieldView.ChangeCoordinatesTo(list, Vector2.zero) + scrollView.scrollOffset;
            pointerStartPosition = evt.position;
            fieldView.CapturePointer(evt.pointerId);

            list.Add(fieldView);
            fieldView.style.position = Position.Absolute;
            fieldView.style.left = fieldView.style.right = 0;
            fieldView.transform.position = targetStartPosition;

            enabled = true;
        }

        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (enabled && target.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - pointerStartPosition;


                fieldView.transform.position = new Vector2(targetStartPosition.x, targetStartPosition.y + pointerDelta.y);

            }
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (enabled && target.HasPointerCapture(evt.pointerId))
            {
                fieldView.ReleasePointer(evt.pointerId);
            }
        }

        private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
        {
            if (enabled)
            {
                UQueryBuilder<VisualElement> allSockets = list.Query<VisualElement>(className: "socket");
                UQueryBuilder<VisualElement> overlappingSocket = allSockets.Where(OverlapsTarget);
                VisualElement closestOverlappingSocket = FindClosestSocket(overlappingSocket);
                if (closestOverlappingSocket != null)
                {
                    if (closestOverlappingSocket != socket)
                    {
                        var otherView = closestOverlappingSocket.Q<FieldView>();
                        socket.Add(otherView);

                        closestOverlappingSocket.Add(fieldView);
                        fieldView.style.position = Position.Relative;
                        fieldView.transform.position = Vector2.zero;

                        enabled = false;

                        fieldView.BehaviorView.SwapObjects(fieldView.AIObject, otherView.AIObject);
                        return;
                    }
                }
                socket.Add(fieldView);
                fieldView.style.position = Position.Relative;
                fieldView.transform.position = Vector2.zero;

                enabled = false;
            }
        }

        private bool OverlapsTarget(VisualElement socketParam)
        {
            return fieldView.worldBound.Overlaps(socketParam.worldBound);
        }

        private VisualElement FindClosestSocket(UQueryBuilder<VisualElement> sockets)
        {
            List<VisualElement> slotsList = sockets.ToList();
            float bestDistanceSq = float.MaxValue;
            VisualElement closest = null;
            foreach (VisualElement slot in slotsList)
            {
                Vector3 displacement = RootSpaceOfSlot(slot) - fieldView.transform.position;
                float distanceSq = displacement.sqrMagnitude;
                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    closest = slot;
                }
            }
            return closest;
        }

        private Vector3 RootSpaceOfSlot(VisualElement slot)
        {
            Vector2 slotWorldSpace = slot.parent.LocalToWorld(slot.layout.position);
            return fieldView.WorldToLocal(slotWorldSpace);
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
