#include <stdlib.h>

#include "memory.h"

/*************************************************************
Behavoir for this method:
oldSize	    newSize	                Operation
0	        Non‑zero	            Allocate new block. (Same as malloc())
Non‑zero	0	                    Free allocation. (Literally free())
Non‑zero	Smaller than oldSize	Shrink existing allocation.
Non‑zero	Larger than oldSize	    Grow existing allocation.
*************************************************************/
void *reallocate(void *pointer, size_t oldSize, size_t newSize)
{
    if (newSize == 0)
    {
        free(pointer);
        return NULL;
    }

    void *result = realloc(pointer, newSize);

    if (result == NULL)
        exit(1);

    return result;
}