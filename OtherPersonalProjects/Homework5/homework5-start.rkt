#lang racket

{provide asum find-indices remove square}


(define (square lst)
  (map (lambda (i) (* i i)) lst)
)  

(define (remove n lst)
  (filter (lambda (i) 
                  (if (eq? n i) #f #t)
          )
  lst)
)  


(define (asum lst)
  (foldl + 0 (map * lst (map (lambda (k) (if (even? k) 1 -1)) (range (length lst)))))
)
  

(define (find-indices n lst)
  
  
  
  (let recurse ((theList lst) (i 0))
    (cond
      ((null? theList)
       '())
      ((equal? n (car theList))
       (append (list i) 
               (recurse (cdr theList) (+ i 1))
               ))
      (else
       (recurse (cdr theList) (+ i 1)))))
)


(remove 1 '(1 1 2 2 3 4 1 5))

(square '(1 2 3 4))

(asum '(1 2 3 4))

(find-indices 1 '(1 2 3 1 4 5 1 10 11 1 100))