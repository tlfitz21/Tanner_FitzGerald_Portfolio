type MultiplicationEntry = {
  left: number
  right: number
  product: number
}

const pool: MultiplicationEntry[] = []

for (let left = 2; left <= 12; left += 1) {
  for (let right = 2; right <= 12; right += 1) {
    pool.push({
      left,
      right,
      product: left * right,
    })
  }
}

export const multiplicationTablePool = pool
