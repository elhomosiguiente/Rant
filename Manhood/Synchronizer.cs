﻿using System;

namespace Manhood
{
    internal class Synchronizer
    {
        private SelectorType _type;
        private int _index;
        private int[] _state;
        private bool _pinned;
        private readonly RNG _rng;

        public Synchronizer(SelectorType type, long seed)
        {
            _type = type;
            _rng = new RNG(seed);
            _index = 0;
            _state = null;
            _pinned = false;
        }

        public bool Pinned
        {
            get { return _pinned; }
            set { _pinned = value; }
        }

        public SelectorType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public void Reset()
        {
            _rng.Reset();
            FillSlots();
        }

        public int NextItem(int count)
        {
            if (_state == null)
            {
                _state = new int[count];
                FillSlots();
            }
            else if (count != _state.Length)
            {
                Array.Resize(ref _state, count);
                FillSlots();
            }

            return Step(false);
        }

        public int Step(bool force)
        {
            if (_type == SelectorType.Uniform) return _state[0];
            if (_index >= _state.Length)
            {
                _index = 0;
                if (_type == SelectorType.Deck) ScrambleSlots();
            }
            if (_pinned && !force) return _state[_index];
            
            return _state[_index++];
        }

        public void FillSlots()
        {
            for (int i = 0; i < _state.Length; _state[i] = i++) { }
            if (Type != SelectorType.Ordered) ScrambleSlots();
        }

        public void ScrambleSlots()
        {
            int t, s;

            for (int i = 0; i < _state.Length; i++)
            {
                t = _state[i];
                do
                {
                    s = _rng.Next(_state.Length);
                } while (s == t && _state.Length < 3);
                _state[i] = _state[s];
                _state[s] = t;
            }
        }
    }
}