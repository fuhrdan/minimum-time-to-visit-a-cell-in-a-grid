//*****************************************************************************
//** 2577. Minimum Time to Visit a Cell In a Grid    leetcode                **
//*****************************************************************************

#define DIRECTIONS 4

typedef struct {
    int cost;
    int x;
    int y;
} Node;

typedef struct {
    Node* heap;
    int size;
    int capacity;
} PriorityQueue;

void initPriorityQueue(PriorityQueue* pq, int capacity) {
    pq->heap = (Node*)malloc(capacity * sizeof(Node));
    pq->size = 0;
    pq->capacity = capacity;
}

void freePriorityQueue(PriorityQueue* pq) {
    free(pq->heap);
}

void push(PriorityQueue* pq, int cost, int x, int y) {
    if (pq->size == pq->capacity) {
        pq->capacity *= 2;
        pq->heap = (Node*)realloc(pq->heap, pq->capacity * sizeof(Node));
    }
    Node newNode = {cost, x, y};
    int i = pq->size++;
    while (i > 0) {
        int parent = (i - 1) / 2;
        if (pq->heap[parent].cost <= cost) break;
        pq->heap[i] = pq->heap[parent];
        i = parent;
    }
    pq->heap[i] = newNode;
}

Node pop(PriorityQueue* pq) {
    Node top = pq->heap[0];
    Node last = pq->heap[--pq->size];
    int i = 0;
    while (i * 2 + 1 < pq->size) {
        int left = i * 2 + 1;
        int right = i * 2 + 2;
        int smallest = (right < pq->size && pq->heap[right].cost < pq->heap[left].cost) ? right : left;
        if (last.cost <= pq->heap[smallest].cost) break;
        pq->heap[i] = pq->heap[smallest];
        i = smallest;
    }
    pq->heap[i] = last;
    return top;
}

int dijkstra(int** grid, int gridSize, int gridColSize, int* start, int* target) {
    int directions[DIRECTIONS][2] = {{1, 0}, {0, 1}, {-1, 0}, {0, -1}};
    int** best = (int**)malloc(gridSize * sizeof(int*));
    for (int i = 0; i < gridSize; i++) {
        best[i] = (int*)malloc(gridColSize * sizeof(int));
        for (int j = 0; j < gridColSize; j++) {
            best[i][j] = INT_MAX;
        }
    }
    best[start[0]][start[1]] = 0;

    PriorityQueue pq;
    initPriorityQueue(&pq, gridSize * gridColSize);
    push(&pq, 0, start[0], start[1]);

    while (pq.size > 0) {
        Node current = pop(&pq);
        int currCost = current.cost, i = current.x, j = current.y;

        if (currCost > best[i][j]) continue;
        if (i == target[0] && j == target[1]) break;

        for (int d = 0; d < DIRECTIONS; d++) {
            int ni = i + directions[d][0], nj = j + directions[d][1];
            if (ni < 0 || ni >= gridSize || nj < 0 || nj >= gridColSize) continue;

            int nextCost = grid[ni][nj] + ((grid[ni][nj] % 2) == (currCost % 2) ? 1 : 0);
            nextCost = (nextCost > currCost + 1) ? nextCost : currCost + 1;

            if (best[ni][nj] > nextCost) {
                best[ni][nj] = nextCost;
                push(&pq, nextCost, ni, nj);
            }
        }
    }

    int result = best[target[0]][target[1]];
    for (int i = 0; i < gridSize; i++) {
        free(best[i]);
    }
    free(best);
    freePriorityQueue(&pq);

    return result;
}

int minimumTime(int** grid, int gridSize, int* gridColSize) {
    if (grid[0][1] > 1 && grid[1][0] > 1) {
        return -1;
    }
    int start[2] = {0, 0};
    int target[2] = {gridSize - 1, gridColSize[0] - 1};
    return dijkstra(grid, gridSize, gridColSize[0], start, target);
}