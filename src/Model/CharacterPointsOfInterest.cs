using UnityEngine;
using System.Linq;

namespace LFE {

    public class CharacterPointsOfInterest {
        public bool IsMale => _dazCharacter?.isMale ?? false;


        // general
        public Vector3 CraniumHeight => RoughVertex(2087);
        public Vector3 ChinHeight => RoughVertex(2079);
        public Vector3 ShoulderHeight => Vector3.Lerp(RoughVertex(11110), RoughVertex(182), 0.5f);
        public Vector3 BustHeight => RoughVertex(10939);
        public Vector3 UnderbustHeight => RoughVertex(21469);
        public Vector3 NavelHeight => Vector3.Lerp(RoughVertex(18824), RoughVertex(8147), 0.5f);
        public Vector3 CrotchHeight => RoughVertex(22208);
        public Vector3 KneeUnderHeight => Vector3.Lerp(RoughVertex(8508), RoughVertex(19179), 0.5f);
        public Vector3 HeelHeight => Vector3.Lerp(_footRightCollider.transform.position, _footLeftCollider.transform.position, 0.5f);

        // face
        public Vector3 CraniumRightSide => RoughVertex(20646);
        public Vector3 CraniumLeftSide => RoughVertex(3236);
        public Vector3 EyeRightOuterSide => RoughVertex(18050);
        public Vector3 EyeRightCenter => _eyeRight.transform.position;
        public Vector3 EyeLeftOuterSide => RoughVertex(7351);
        public Vector3 EyeLeftCenter => _eyeLeft.transform.position;
        public Vector3 NoseBottomHeight => RoughVertex(3252);
        public Vector3 NoseTip => RoughVertex(2111);
        public Vector3 MouthCenterHeight => Vector3.Lerp(RoughVertex(2136), RoughVertex(2145), 0.5f);
        public Vector3 MouthRightSide => IsMale ? Vector3.zero : RoughVertex(12319);
        public Vector3 MouthLeftSide => IsMale ? Vector3.zero : RoughVertex(1655);


        // penis
        public Vector3 PenisTip => IsMale ? RoughVertex(21627) : Vector3.zero;
        public Vector3 PenisBase => IsMale ? Vector3.Lerp(RoughVertex(22270), RoughVertex(22865), 0.5f) : Vector3.zero;
        public Vector3 PenisShaftLeft => IsMale ? RoughVertex(22608) : Vector3.zero;
        public Vector3 PenisShaftRight => IsMale ? RoughVertex(22616) : Vector3.zero;

        // circumferences
        public Vector3[] BustPoints {
            get {
                if(IsMale) {
                    return new Vector3[0];
                }
                return new Vector3[] {
                    Vector3.Lerp(RoughVertex(7213), RoughVertex(17920), 0.5f), // midchest 1/2 way between the nipples at bust height
                    RoughVertex(17920), // bust -- right nipple just to the left
                    RoughVertex(10939), // bust -- right nipple just to the right
                    RoughVertex(19588),
                    RoughVertex(19617), // outside bust curve 1
                    RoughVertex(13233), // outside bust curve 2
                    RoughVertex(11022), // bust -- right back
                    Vector3.Lerp(RoughVertex(10495), RoughVertex(10895), 0.5f), // bust -- back center
                    // now the same as above but backwards - only really need to know 1/2 of the vertices and then double it up
                    RoughVertex(11022),
                    RoughVertex(13233),
                    RoughVertex(19617),
                    RoughVertex(19588),
                    RoughVertex(10939),
                    RoughVertex(17920),
                    Vector3.Lerp(RoughVertex(7213), RoughVertex(17920), 0.5f)
                };
            }
        }

        public Vector3[] UnderbustPoints {
            get {
                if(IsMale) {
                    return new Vector3[0];
                }
                return new Vector3[] {
                    Vector3.Lerp(RoughVertex(10822), RoughVertex(10820), 0.7f), // mid chest
                    Vector3.Lerp(RoughVertex(21466), RoughVertex(21207), 0.40f),
                    RoughVertex(21469), // right breast under nipple
                    RoughVertex(21470), // right breast under nipple
                    Vector3.Lerp(RoughVertex(21471), RoughVertex(21307), 0.12f),
                    Vector3.Lerp(RoughVertex(21370), RoughVertex(21424), 0.45f),
                    RoughVertex(21394), // right side 
                    Vector3.Lerp(RoughVertex(11022), RoughVertex(21508), 0.4f),
                    Vector3.Lerp(RoughVertex(10859), RoughVertex(2100), 0.5f), // back
                    // now the same as above but backwards - only really need to know 1/2 of the vertices and then double it up
                    Vector3.Lerp(RoughVertex(11022), RoughVertex(21508), 0.4f),
                    RoughVertex(21394), // right side 
                    Vector3.Lerp(RoughVertex(21370), RoughVertex(21424), 0.45f),
                    Vector3.Lerp(RoughVertex(21471), RoughVertex(21307), 0.12f),
                    RoughVertex(21470), // right breast under nipple
                    RoughVertex(21469), // right breast under nipple
                    Vector3.Lerp(RoughVertex(21466), RoughVertex(21207), 0.40f),
                    Vector3.Lerp(RoughVertex(10822), RoughVertex(10820), 0.7f), // mid chest
                };
            }
        }

        public Vector3[] WaistPoints {
            get {
                if(IsMale) {
                    return new Vector3[] {
                        RoughVertex(10812),
                        RoughVertex(21460),
                        RoughVertex(21518),
                        Vector3.Lerp(RoughVertex(21064),RoughVertex(13727),0.2f),
                        RoughVertex(14855),
                        RoughVertex(2921),
                        // now backwards
                        RoughVertex(14855),
                        Vector3.Lerp(RoughVertex(21064),RoughVertex(13727),0.2f),
                        RoughVertex(21518),
                        RoughVertex(21460),
                        RoughVertex(10812),
                    };
                }
                else {
                    return new Vector3[] {
                        RoughVertex(8152), // front and center
                        RoughVertex(19663), // front right 1
                        RoughVertex(13675), // front right 2
                        RoughVertex(13715), // front right 3
                        RoughVertex(13727), // right side
                        RoughVertex(13725), // back curve 1
                        RoughVertex(2921), // back
                        // now backwards
                        RoughVertex(13725), // back curve 1
                        RoughVertex(13727), // right side
                        RoughVertex(13715), // front right 3
                        RoughVertex(13675), // front right 2
                        RoughVertex(19663), // front right 1
                        RoughVertex(8152) // front and center
                    };
                }
            }
        }

        public Vector3[] HipPoints {
            get {
                if(IsMale) {
                    return new Vector3[] {
                        Vector3.Lerp(RoughVertex(22710), RoughVertex(22700), 0.5f), // front and center
                        RoughVertex(13750), // front right 1
                        RoughVertex(18460), // front right 2
                        Vector3.Lerp(RoughVertex(11234), RoughVertex(18491), 0.8f), // front right 3
                        RoughVertex(18529), // glute curve 1
                        Vector3.Lerp(RoughVertex(18555), RoughVertex(7875), 0.5f), // glute middle
                        // now backwards
                        RoughVertex(18529), // glute curve 1
                        Vector3.Lerp(RoughVertex(11234), RoughVertex(18491), 0.8f), // front right 3
                        RoughVertex(18460), // front right 2
                        RoughVertex(13750), // front right 1
                        Vector3.Lerp(RoughVertex(22710), RoughVertex(22700), 0.5f), // front and center
                    };
                }
                else {
                    return new Vector3[] {
                        RoughVertex(22843), // front and center
                        RoughVertex(13750), // front right 1
                        RoughVertex(18460), // front right 2
                        Vector3.Lerp(RoughVertex(11234), RoughVertex(18491), 0.8f), // front right 3
                        RoughVertex(18512), // right side
                        RoughVertex(18529), // glute curve 1
                        RoughVertex(18562), // glute curve 2
                        Vector3.Lerp(RoughVertex(18562), RoughVertex(7878), 0.5f), // glute middle
                        // now backwards
                        RoughVertex(18562), // glute curve 2
                        RoughVertex(18529), // glute curve 1
                        RoughVertex(18512), // right side
                        Vector3.Lerp(RoughVertex(11234), RoughVertex(18491), 0.8f), // front right 3
                        RoughVertex(18460), // front right 2
                        RoughVertex(13750), // front right 1
                        RoughVertex(22843), // front and center
                    };
                }

            }

        }

        Atom _person;
        DAZCharacter _dazCharacter;
        DAZSkinV2 _dazSkin;
        DAZBone _eyeLeft;
        DAZBone _eyeRight;
        CapsuleCollider _footRightCollider;
        CapsuleCollider _footLeftCollider;

        public Atom Person {
            get {
                return _person;
            }
            set {
                _person = value;
                _dazCharacter = _person?.GetComponentInChildren<DAZCharacter>();
                _dazSkin = _dazCharacter?.skin;
                _eyeLeft = _person?.GetStorableByID("lEye") as DAZBone ?? null;
                _eyeRight = _person?.GetStorableByID("rEye") as DAZBone ?? null;
                _footRightCollider = _person?.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("rFoot/_Collider1")) ?? null; 
                _footLeftCollider = _person?.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("lFoot/_Collider1")) ?? null; 
            }
        }

        public bool HasSkin => _dazSkin != null;

        public CharacterPointsOfInterest(Atom person) {
            Person = person;
        }

        private Vector3 RoughVertex(int i) {
            return _dazSkin.rawSkinnedVerts[i];
        }

        private static string ColliderName(Collider collider)  {
            var parent = collider.attachedRigidbody != null ? collider.attachedRigidbody.name : collider.transform.parent.name;
            var label = parent == collider.name ? collider.name : $"{parent}/{collider.name}";
            return label;
        }
    }
}