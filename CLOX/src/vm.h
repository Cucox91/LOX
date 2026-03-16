#ifndef clox_vm_h
#define clox_vm_h

#include "chunk.h"

#define STACK_MAX 256

/*******************************************************************************
 * When the stack is empty we will point to -1. This is an undefined in C.
 * When the stack is full is allowed too to have a pointer that is past the end
 * of the array.
 * Remeber: The pointer always points to the first available empty memory addr.
 ******************************************************************************/

typedef struct
{
    Chunk *chunk;
    uint8_t *ip;            // Instruction Pointer.
    Value stack[STACK_MAX]; // Here we have the full stack array.
    Value *stackTop;        // A pointer (index) to the top empty stack's elem.
} VM;

typedef enum
{
    INTERPRET_OK,
    INTERPRET_COMPILE_ERROR,
    INTERPRET_RUNTIME_ERROR
} InterpretResult;

void initVM();
void freeVM();

InterpretResult interpret(Chunk *cshunk);

#endif