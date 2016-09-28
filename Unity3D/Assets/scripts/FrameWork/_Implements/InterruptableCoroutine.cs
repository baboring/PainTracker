using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HC
{
    // 중단 가능한 코루틴 실행자
    public class InterruptableCoroutine
    {
        IEnumerator enumerator;
        MonoBehaviour _behaviour;

        // YieldInstructions를 분리된 코루틴으로서 실행할 수 있게 해주는 코루틴 정보
        private class CoroutineInfo {
            public YieldInstruction instruction;
            public bool done;
        }

        // Yield Instruction 하나를 실행하는 코루틴.
        // 코루틴 실행에 필요한 정보 패킷
        IEnumerator YieldInstructionCoroutine(CoroutineInfo info)  {
            info.done = false;
            yield return info.instruction;
            info.done = true;
        }

        // yield instruction을 기다린다.
        // 실행할 코루틴
        // 실행할 Instruction
        IEnumerator WaitForCoroutine(YieldInstruction instruction)
        {
            var ci = new CoroutineInfo { instruction = instruction, done = false };
            _behaviour.StartCoroutine(YieldInstructionCoroutine(ci));
            while (!ci.done)
                yield return null;
        }

        IEnumerator Run()
        {
            //무한루프
            while (true)
            {
                //현재 코루틴이 존재하는지 체크
                if (enumerator != null)
                {
                    //코루틴이 중간에 변하는것에 대비해 카피한다.
                    var enm = enumerator;
                    //코루틴의 다음 단계를 실행
                    var valid = enumerator.MoveNext();
                    //이뉴머레이터가 바뀌었는지 체크한다.
                    if (enm == enumerator)
                    {
                        //같은 이뉴머레이터라면
                        if (enumerator != null)
                        {
                            //yield 실행후 결과를 가져온다.
                            var result = enumerator.Current;
                            //코루틴 여부 확인
                            if (result is IEnumerator)
                            {
                                //현재 코루틴을 푸시한 후 새 코루틴을 실행
                                _stack.Push(enumerator);
                                enumerator = result as IEnumerator;
                                yield return null;
                            }
                            //yield instruction인지 체크
                            else if (result is YieldInstruction)
                            {
                                //yield instructions의 중단을 위해서는
                                //따로 코루틴으로서 실행하여
                                //기다려야 할 필요가 있다.
                                _stack.Push(enumerator);
                                //yieldinstruction을 기다리는 코루틴을 만든다.
                                enumerator = WaitForCoroutine(result as YieldInstruction);
                                yield return null;
                            }
                            else {
                                //아니면 값을 리턴
                                yield return enumerator.Current;
                            }
                        }
                        else {
                            //이뉴머레이터가 null이었다면
                            //invalid 이뉴머레이터로 마킹한다.
                            valid = false;
                            yield return null;
                        }
                        //valid state인지 체크
                        if (!valid)
                        {
                            //아니라면 스택에 쌓인 코루틴이 있는지 체크.
                            if (_stack.Count >= 1)
                            {
                                //스택에 있는 코루틴을 다시 가져온다.
                                enumerator = _stack.Pop();
                            }
                            else {
                                //이 이뉴머레이터를 다시 이용하지 않도록 한다.
                                enumerator = null;
                            }
                        }
                    }
                    else {
                        //이뉴머레이터가 바뀌었으면 그냥 yield
                        yield return null;
                    }
                }
                else {
                    //만약 이뉴머레이터가 null이라면 그냥 yield
                    yield return null;
                }
            }
        }

        // 모노 위에서 동작하는 제어 형 코루틴이다.
        public InterruptableCoroutine(MonoBehaviour behaviour)
        {
            _behaviour = behaviour;
            _behaviour.StartCoroutine(Run());
        }

        // 스텍 구조로 코루틴을 실행 하자.
        Stack<IEnumerator> _stack = new Stack<IEnumerator>();

        // 특정한 코루틴을 호출.
        // 호출할 코루틴
        public void Call(IEnumerator enm)
        {
            _stack.Push(enumerator);
            enumerator = enm;
        }

		public StoppableCoroutine Call(IEnumerable enm) {
			StoppableCoroutine co_rtn = enm.MakeStoppable();
			Logger.Assert(null != co_rtn, "coroutine is null : " + enm);

			_stack.Push(co_rtn);
			enumerator = co_rtn;

			return co_rtn;
		}



		// 부수적인 스택에서 특정 코루틴을 실행.
		// 실행할 코루틴
		// 이 코루틴을 위해 이용될 스택.
		public void Run(IEnumerator enm, Stack<IEnumerator> stack = null)
        {
            enumerator = enm;
            if (stack != null)
            {
                _stack = stack;
            }
            else {
                _stack.Clear();
            }
        }

        // 실행중인 코루틴을 위해 새 스택을 제작
        // 스택
        public Stack<IEnumerator> CreateStack()
        {
            var current = _stack;
            _stack = new Stack<IEnumerator>();
            return current;
        }

        // 현재 코루틴 취소
        public void Cancel()
        {
            enumerator = null;
            _stack.Clear();
        }
    }

}