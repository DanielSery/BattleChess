## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.New_Int()
       push      rbx
       sub       rsp,20
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+10]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r11b
       movzx     r11d,r11b
       cmp       ebx,40
       setae     r9b
       movzx     r9d,r9b
       or        r9d,r11d
       jne       short M00_L03
M00_L01:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,20
       pop       rbx
       ret
M00_L03:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r9b
       movzx     r9d,r9b
       cmp       ebx,40
       setae     r11b
       movzx     r11d,r11b
       or        r9d,r11d
       jne       short M00_L06
M00_L05:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 218
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.New_Byte()
       push      rbx
       sub       rsp,20
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+8]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r11b
       movzx     r11d,r11b
       cmp       ebx,40
       setae     r9b
       movzx     r9d,r9b
       or        r9d,r11d
       jne       short M00_L03
M00_L01:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,20
       pop       rbx
       ret
M00_L03:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r9b
       movzx     r9d,r9b
       cmp       ebx,40
       setae     r11b
       movzx     r11d,r11b
       or        r9d,r11d
       jne       short M00_L06
M00_L05:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 220
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.Old_Int()
       push      rsi
       push      rbx
       sub       rsp,28
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+10]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L03
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L03
       mov       r9d,ebx
M00_L01:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M00_L03:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L06
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L06
       mov       r9d,ebx
M00_L05:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 206
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.Old_Byte()
       push      rsi
       push      rbx
       sub       rsp,28
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+8]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L03
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L03
       mov       r9d,ebx
M00_L01:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M00_L03:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L06
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L06
       mov       r9d,ebx
M00_L05:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 208
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.New_Int()
       push      rbx
       sub       rsp,20
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+10]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r11b
       movzx     r11d,r11b
       cmp       ebx,40
       setae     r9b
       movzx     r9d,r9b
       or        r9d,r11d
       jne       short M00_L03
M00_L01:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,20
       pop       rbx
       ret
M00_L03:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r9b
       movzx     r9d,r9b
       cmp       ebx,40
       setae     r11b
       movzx     r11d,r11b
       or        r9d,r11d
       jne       short M00_L06
M00_L05:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 218
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.New_Byte()
       push      rbx
       sub       rsp,20
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+8]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r11b
       movzx     r11d,r11b
       cmp       ebx,40
       setae     r9b
       movzx     r9d,r9b
       or        r9d,r11d
       jne       short M00_L03
M00_L01:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,20
       pop       rbx
       ret
M00_L03:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r9b
       movzx     r9d,r9b
       cmp       ebx,40
       setae     r11b
       movzx     r11d,r11b
       or        r9d,r11d
       jne       short M00_L06
M00_L05:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 220
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.Old_Int()
       push      rsi
       push      rbx
       sub       rsp,28
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+10]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L03
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L03
       mov       r9d,ebx
M00_L01:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M00_L03:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L06
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L06
       mov       r9d,ebx
M00_L05:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 206
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.Old_Byte()
       push      rsi
       push      rbx
       sub       rsp,28
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+8]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L03
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L03
       mov       r9d,ebx
M00_L01:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M00_L03:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L06
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L06
       mov       r9d,ebx
M00_L05:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 208
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.New_Int()
       push      rbx
       sub       rsp,20
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+10]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r11b
       movzx     r11d,r11b
       cmp       ebx,40
       setae     r9b
       movzx     r9d,r9b
       or        r9d,r11d
       jne       short M00_L03
M00_L01:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,20
       pop       rbx
       ret
M00_L03:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r9b
       movzx     r9d,r9b
       cmp       ebx,40
       setae     r11b
       movzx     r11d,r11b
       or        r9d,r11d
       jne       short M00_L06
M00_L05:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 218
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.New_Byte()
       push      rbx
       sub       rsp,20
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+8]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r11b
       movzx     r11d,r11b
       cmp       ebx,40
       setae     r9b
       movzx     r9d,r9b
       or        r9d,r11d
       jne       short M00_L03
M00_L01:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,20
       pop       rbx
       ret
M00_L03:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       xor       r11d,ebx
       sar       r11d,3
       sar       r9d,8
       lea       ebx,[rbx+r9*8]
       test      r11d,r11d
       setne     r9b
       movzx     r9d,r9b
       cmp       ebx,40
       setae     r11b
       movzx     r11d,r11b
       or        r9d,r11d
       jne       short M00_L06
M00_L05:
       add       eax,ebx
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       ebx,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 220
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.Old_Int()
       push      rsi
       push      rbx
       sub       rsp,28
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+10]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L03
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L03
       mov       r9d,ebx
M00_L01:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M00_L03:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       mov       r11d,[rcx+r9*4+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L06
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L06
       mov       r9d,ebx
M00_L05:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 206
```

## .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```assembly
; CrownsGuard.Benchmarks.PositionsHelperBenchmarks.Old_Byte()
       push      rsi
       push      rbx
       sub       rsp,28
       xor       eax,eax
       mov       rdx,[rcx+18]
       mov       rcx,[rcx+8]
       xor       r8d,r8d
       mov       r10d,[rcx+8]
       test      r10d,r10d
       jle       short M00_L02
       test      rdx,rdx
       je        short M00_L04
       cmp       [rdx+8],r10d
       jl        short M00_L04
M00_L00:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L03
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L03
       mov       r9d,ebx
M00_L01:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L00
M00_L02:
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M00_L03:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L01
M00_L04:
       mov       r9d,r8d
       movzx     r11d,byte ptr [rcx+r9+10]
       cmp       r8d,[rdx+8]
       jae       short M00_L07
       movsx     r9,word ptr [rdx+r9*2+10]
       movsx     rbx,r9b
       add       ebx,r11d
       sar       r11d,3
       mov       esi,ebx
       sar       esi,3
       cmp       r11d,esi
       jne       short M00_L06
       sar       r9d,5
       and       r9d,0FFFFFFF8
       add       ebx,r9d
       cmp       ebx,40
       jae       short M00_L06
       mov       r9d,ebx
M00_L05:
       add       eax,r9d
       inc       r8d
       cmp       r10d,r8d
       jg        short M00_L04
       jmp       short M00_L02
M00_L06:
       mov       r9d,0FFFFFFFF
       jmp       short M00_L05
M00_L07:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 208
```

