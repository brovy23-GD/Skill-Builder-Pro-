using System;
using System.Collections.Generic;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Algorithms
{
    /// <summary>
    /// SortingAlgorithms.cs - Collection of sorting algorithms for drill management
    ///
    /// WHAT IT DOES:
    /// - Provides four different sorting algorithms to sort lists of drills
    /// - Each algorithm sorts the list in place (modifies the original list)
    /// - Each algorithm accepts a compare function so you can sort by any property
    ///
    /// WHEN TO USE EACH:
    /// - SelectionSort: small lists, simple to understand, always O(n^2)
    /// - InsertionSort: small or nearly sorted lists, fast when data is almost sorted
    /// - BubbleSort:    learning purposes, has early exit optimization
    /// - QuickSort:     large lists, best general purpose sort, O(n log n) average
    ///
    /// HOW THE COMPARE FUNCTION WORKS:
    /// - Pass in a function that compares two drills
    /// - Returns negative if first drill should come BEFORE second
    /// - Returns zero if they are equal
    /// - Returns positive if first drill should come AFTER second
    ///
    /// EXAMPLE COMPARE FUNCTIONS:
    /// - Sort by difficulty: (a, b) => a.Difficulty - b.Difficulty
    /// - Sort by duration:   (a, b) => a.Duration - b.Duration
    /// - Sort by name:       (a, b) => string.Compare(a.Name, b.Name)
    ///
    /// TIME COMPLEXITY SUMMARY:
    /// - SelectionSort: O(n^2) always
    /// - InsertionSort: O(n^2) average, O(n) best case on nearly sorted data
    /// - BubbleSort:    O(n^2) average, O(n) best case with early exit
    /// - QuickSort:     O(n log n) average, O(n^2) worst case
    ///
    /// SPACE COMPLEXITY:
    /// - SelectionSort: O(1)
    /// - InsertionSort: O(1)
    /// - BubbleSort:    O(1)
    /// - QuickSort:     O(log n) for the recursion call stack
    /// </summary>
    public static class SortingAlgorithms
    {
        // ═══════════════════════════════════════════════════
        // SELECTION SORT
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Selection Sort - finds the smallest item and moves it to the front
        /// Repeats for each position until the whole list is sorted
        ///
        /// HOW IT WORKS:
        /// 1. Divide the list into a sorted region (left) and unsorted region (right)
        /// 2. Find the minimum item in the unsorted region
        /// 3. Swap it to the end of the sorted region
        /// 4. Expand the sorted region by one
        /// 5. Repeat until all items are sorted
        ///
        /// VISUAL EXAMPLE with difficulty values [3, 1, 2, 1]:
        /// Pass 1: find min=1 at index 1, swap with index 0 → [1, 3, 2, 1]
        /// Pass 2: find min=1 at index 3, swap with index 1 → [1, 1, 2, 3]
        /// Pass 3: find min=2 at index 2, already in place  → [1, 1, 2, 3]
        /// Result: [1, 1, 2, 3]
        ///
        /// BEST FOR: small lists under 20 items, simple and easy to understand
        ///
        /// TIME COMPLEXITY: O(n^2) always - does n x (n-1) / 2 comparisons every time
        /// SPACE COMPLEXITY: O(1) - sorts in place using only a temp variable
        /// </summary>
        public static void SelectionSort(
            List<Drill> drills,              // the list to sort (modified in place)
            Func<Drill, Drill, int> compare) // function that compares two drills
        {
            int n = drills.Count; // total number of drills in the list

            // Outer loop: move the sorted region boundary forward one step at a time
            for (int i = 0; i < n - 1; i++)
            {
                // Assume the first item in the unsorted region is the minimum
                int minIndex = i;

                // Inner loop: search the rest of the unsorted region for a smaller item
                for (int j = i + 1; j < n; j++)
                {
                    // If drills[j] is smaller than current minimum update the minimum index
                    if (compare(drills[j], drills[minIndex]) < 0)
                    {
                        minIndex = j; // found a new minimum item
                    }
                }

                // Only swap if the minimum is not already in the correct position
                if (minIndex != i)
                {
                    var temp = drills[i];           // save the current item
                    drills[i] = drills[minIndex];   // move minimum to sorted position
                    drills[minIndex] = temp;         // put saved item where minimum was
                }
            }
        }

        // ═══════════════════════════════════════════════════
        // INSERTION SORT
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Insertion Sort - builds a sorted list one item at a time
        /// Picks each item and inserts it into the correct position in the sorted region
        ///
        /// HOW IT WORKS:
        /// 1. Start with the second item - first item is already considered sorted
        /// 2. Save the current item as the key
        /// 3. Shift all larger sorted items one position to the right to make room
        /// 4. Insert the key into the gap that was created
        /// 5. Move to the next item and repeat
        ///
        /// VISUAL EXAMPLE with difficulty values [3, 1, 2, 1]:
        /// Start:  [3] | [1, 2, 1]
        /// Pass 1: key=1, shift 3 right → [1, 3] | [2, 1]
        /// Pass 2: key=2, shift 3 right → [1, 2, 3] | [1]
        /// Pass 3: key=1, shift 3 and 2 right → [1, 1, 2, 3]
        /// Result: [1, 1, 2, 3]
        ///
        /// BEST FOR: nearly sorted lists (very fast), small lists
        ///
        /// TIME COMPLEXITY: O(n^2) average, O(n) best case when already sorted
        /// SPACE COMPLEXITY: O(1) - sorts in place using only a key variable
        /// </summary>
        public static void InsertionSort(
            List<Drill> drills,              // the list to sort (modified in place)
            Func<Drill, Drill, int> compare) // function that compares two drills
        {
            // Start at index 1 because a single item at index 0 is already sorted
            for (int i = 1; i < drills.Count; i++)
            {
                // Save the current item we are trying to insert into sorted position
                var key = drills[i];

                // Start comparing from the item just before current position
                int j = i - 1;

                // Shift all sorted items that are larger than the key one spot to the right
                // This creates the gap we need to insert the key
                while (j >= 0 && compare(drills[j], key) > 0)
                {
                    drills[j + 1] = drills[j]; // shift this item one position right
                    j--;                        // move one position left to check next
                }

                // Insert the key into its correct position
                // j + 1 is where the gap ended up after all the shifting
                drills[j + 1] = key;
            }
        }

        // ═══════════════════════════════════════════════════
        // BUBBLE SORT
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Bubble Sort - repeatedly swaps adjacent items that are in the wrong order
        /// Larger items gradually bubble up to the end of the list with each pass
        ///
        /// HOW IT WORKS:
        /// 1. Start at the beginning of the list
        /// 2. Compare each pair of adjacent items
        /// 3. If they are in the wrong order swap them
        /// 4. After one full pass the largest item is at the end
        /// 5. Repeat but skip the last sorted item each time
        /// 6. OPTIMIZATION: if no swaps happened the list is sorted so stop early
        ///
        /// VISUAL EXAMPLE with difficulty values [3, 1, 2, 1]:
        /// Pass 1: 3>1 swap→[1,3,2,1], 3>2 swap→[1,2,3,1], 3>1 swap→[1,2,1,3]
        /// Pass 2: 1<2 ok, 2>1 swap→[1,1,2,3], 2<3 ok
        /// Pass 3: no swaps → EARLY EXIT
        /// Result: [1, 1, 2, 3]
        ///
        /// BEST FOR: learning purposes, detecting nearly sorted data with early exit
        ///
        /// TIME COMPLEXITY: O(n^2) average, O(n) best case with early exit
        /// SPACE COMPLEXITY: O(1) - sorts in place using only a temp variable
        /// </summary>
        public static void BubbleSort(
            List<Drill> drills,              // the list to sort (modified in place)
            Func<Drill, Drill, int> compare) // function that compares two drills
        {
            int n = drills.Count; // total number of drills

            // Outer loop: each pass puts one more item in its final position
            for (int i = 0; i < n - 1; i++)
            {
                // Track whether any swaps happened during this pass
                bool swapped = false;

                // Inner loop: compare adjacent pairs in the unsorted region
                // We stop at n - i - 1 because the last i items are already sorted
                for (int j = 0; j < n - i - 1; j++)
                {
                    // If the left item should come after the right item they are out of order
                    if (compare(drills[j], drills[j + 1]) > 0)
                    {
                        // Swap the two adjacent items
                        var temp = drills[j];           // save the left item
                        drills[j] = drills[j + 1];      // move right item to the left
                        drills[j + 1] = temp;           // put saved item on the right
                        swapped = true;                  // record that a swap happened
                    }
                }

                // EARLY EXIT OPTIMIZATION
                // If no swaps happened this entire pass the list is already fully sorted
                // No need to keep going so we break out early
                if (!swapped)
                    break;
            }
        }

        // ═══════════════════════════════════════════════════
        // QUICK SORT
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Quick Sort - fastest general purpose sorting algorithm for most cases
        /// Divides the list around a pivot and recursively sorts each half
        ///
        /// HOW IT WORKS:
        /// 1. Pick a pivot element (we always use the last item)
        /// 2. Partition: move all items smaller than pivot to the left
        ///               move all items larger than pivot to the right
        /// 3. The pivot is now in its final correct position
        /// 4. Recursively sort the left half (items smaller than pivot)
        /// 5. Recursively sort the right half (items larger than pivot)
        /// 6. Base case: a section with 0 or 1 items is already sorted
        ///
        /// VISUAL EXAMPLE with difficulty values [3, 1, 2, 1]:
        /// Pivot = 1 (last item)
        /// Partition: no items less than 1, place pivot → [1, 3, 2, 1] ... recurse
        /// After full recursion: [1, 1, 2, 3]
        ///
        /// BEST FOR: large lists of 100 or more items, general purpose use
        ///
        /// TIME COMPLEXITY: O(n log n) average, O(n^2) worst case on already sorted data
        /// SPACE COMPLEXITY: O(log n) for the recursion call stack
        /// </summary>
        public static void QuickSort(
            List<Drill> drills,              // the list to sort (modified in place)
            Func<Drill, Drill, int> compare) // function that compares two drills
        {
            // Kick off the recursive sort on the full list
            // low = first index (0), high = last index (Count - 1)
            QuickSortHelper(drills, 0, drills.Count - 1, compare);
        }

        /// <summary>
        /// QuickSort recursive helper - sorts a section of the list
        ///
        /// HOW IT WORKS:
        /// 1. If the section has 1 or 0 items it is already sorted (base case - stop)
        /// 2. Partition the section around a pivot and get the pivot's final index
        /// 3. Recursively sort everything to the left of the pivot
        /// 4. Recursively sort everything to the right of the pivot
        ///
        /// TIME COMPLEXITY: O(n log n) average, O(n^2) worst case
        /// SPACE COMPLEXITY: O(log n) for the recursion stack
        /// </summary>
        private static void QuickSortHelper(
            List<Drill> drills,              // the list we are sorting
            int low,                         // start index of the section to sort
            int high,                        // end index of the section to sort
            Func<Drill, Drill, int> compare) // comparison function
        {
            // Base case: if section has 1 or 0 items it is already sorted so stop
            if (low < high)
            {
                // Partition the section and get the final position of the pivot
                // After partitioning: everything left of pi is smaller than pivot
                //                    everything right of pi is larger than pivot
                int pi = Partition(drills, low, high, compare);

                // Recursively sort the left side (items smaller than pivot)
                QuickSortHelper(drills, low, pi - 1, compare);

                // Recursively sort the right side (items larger than pivot)
                QuickSortHelper(drills, pi + 1, high, compare);
            }
        }

        /// <summary>
        /// Partition helper - rearranges items in a section around the pivot
        /// After this runs the pivot is in its final correct sorted position
        ///
        /// HOW IT WORKS:
        /// 1. Pick the last item as the pivot
        /// 2. Loop through all items except the pivot
        /// 3. If an item is smaller than pivot swap it to the left side
        /// 4. Place pivot in its correct final position
        /// 5. Return the pivot index
        ///
        /// TIME COMPLEXITY: O(n) - loops through the section once
        /// SPACE COMPLEXITY: O(1) - only uses a temp variable for swaps
        /// </summary>
        private static int Partition(
            List<Drill> drills,              // the list we are partitioning
            int low,                         // start index of the section
            int high,                        // end index of the section
            Func<Drill, Drill, int> compare) // comparison function
        {
            // Use the last item as the pivot value
            var pivot = drills[high];

            // i tracks the boundary between smaller and larger items
            // starts just before the section
            int i = low - 1;

            // Loop through all items except the pivot
            for (int j = low; j < high; j++)
            {
                // If this item is smaller than pivot move it to the left side
                if (compare(drills[j], pivot) < 0)
                {
                    i++; // expand the left side boundary

                    // Swap drills[i] and drills[j]
                    var temp = drills[i];
                    drills[i] = drills[j];
                    drills[j] = temp;
                }
            }

            // Place the pivot right after the left side boundary
            var tempPivot = drills[i + 1];
            drills[i + 1] = drills[high];
            drills[high] = tempPivot;

            return i + 1; // return the pivot's final position
        }
    }
}
