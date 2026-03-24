#ifndef clox_value_h
#define clox_value_h

#include "common.h"

typedef struct Obj Obj; // Forward declaration of a Struct.
typedef struct ObjString ObjString;

typedef enum
{
    VAL_BOOL,
    VAL_NIL,
    VAL_NUMBER,
    VAL_OBJ
} ValueType;

/*
Explanation for Union.
This is a way to do optimization on C. It will allow the bytes for each element
defined inside the union to overlap. This will save space waste.
The idea is that the size of the union will be the size of the largest field in
bytes. In this case the size of a double for now.

We need to be very careful because this is very error prompt and MEMORY UNSAFE.
*/
typedef struct
{
    ValueType type;
    union
    {
        bool boolean;
        double number;
        Obj *obj; // Pointer to the HEAP.
    } as;         // 'as' is the name of the union. Plain an simple.
} Value;

/*
    To move between realms you need to understand.
    The _VAL moves from C to LOX.
    The _AS moves from LOX to C.
*/

#define IS_BOOL(value) ((value).type == VAL_BOOL)
#define IS_NIL(value) ((value).type == VAL_NIL)
#define IS_NUMBER(value) ((value).type == VAL_NUMBER)
#define IS_OBJ(value) ((value).type == VAL_OBJ)

#define AS_BOOL(value) ((value).as.boolean)
#define AS_NUMBER(value) ((value).as.number)
#define AS_OBJ(value) ((value).as.obj)

#define BOOL_VAL(value) ((Value){VAL_BOOL, {.boolean = value}})
#define NIL_VAL ((Value){VAL_NIL, {.number = 0}})
#define NUMBER_VAL(value) ((Value){VAL_NUMBER, {.number = value}})
#define OBJ_VAL(value) ((Value){VAL_OBJ, {.obj = (Obj *)object}})

typedef struct
{
    int capacity;
    int count;
    Value *values;
} ValueArray;

bool valuesEqual(Value a, Value b);
void initValueArray(ValueArray *array);
void writeValueArray(ValueArray *array, Value value);
void freeValueArray(ValueArray *array);
#endif