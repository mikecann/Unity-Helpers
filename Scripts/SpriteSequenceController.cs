using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityHelpers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSequenceController : MonoBehaviour
    {
        public List<SpriteSequence> sequences;
        public string sequence;
        public int frame;

        private SpriteRenderer spriteRenderer;
        private SpriteSequence spriteSequence;
        private int lastFrame;
        private string lastSequence;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if(lastSequence!=sequence)
            {
                lastSequence = sequence;
                spriteSequence = sequences.FirstOrDefault(s => s.name == sequence);
                lastFrame = -1;
            }

            if(spriteSequence!=null)
            {
                if (lastFrame != frame && frame != -1)
                {
                    lastFrame = frame;             
                    spriteRenderer.sprite = spriteSequence.sprites[frame];
                }
            }
        }

        public void SetSequence(string sequence)
        {
            this.sequence = sequence;
        }

        public void SetFrame(int frame)
        {
            this.frame = frame;
        }
    }
}
