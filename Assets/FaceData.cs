using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using System.Linq;
using System;

namespace Azw.Iyi
{

    [RequireComponent(typeof(ARFace))]
    public class FaceData : MonoBehaviour
    {
        ARFace face;
        Text text;

        ARKitFaceSubsystem arKitFaceSubsystem;

        UDPSender udp;

        void Awake()
        {
            udp = new UDPSender(GeneralSettings.DistinationIP, GeneralSettings.DistinationPort);
            face = GetComponent<ARFace>();
        }
        void Start() {
            text = GameObject.Find("Text").GetComponent<Text>();
        }

        void OnEnable()
        {
            var faceManager = FindObjectOfType<ARFaceManager>();
            if (faceManager != null)
            {
                arKitFaceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
            }
            face.updated += OnUpdated;
        }

        void OnDisable()
        {
            face.updated -= OnUpdated;
        }

        void OnDestroy()
        {
            udp.Close();
        }

        void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
        {
            using (var blendShapes = arKitFaceSubsystem.GetBlendShapeCoefficients(face.trackableId, Allocator.Temp))
            {
                var list = blendShapes.Select(blendShapeCoefficient => (Enum.GetName(typeof(ARKitBlendShapeLocation), blendShapeCoefficient.blendShapeLocation), blendShapeCoefficient.coefficient)).ToList();
                udp.Send(transform, eventArgs.face, list);

                var t = "";
                foreach (var c in list)
                {
                    t += c.Item1 + c.Item2.ToString() + "\n";
                }
                text.text = t;
            }
        }
    }
}