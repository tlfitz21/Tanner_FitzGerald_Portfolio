#lang racket

(require racket/cmdline)
(require "homework5.rkt")
(define args (current-command-line-arguments))
(define infile (vector-ref args 0))
(define outfile (vector-ref args 1))
(define out (open-output-file outfile #:exists 'replace) )

(for ([line (file->list infile)])
  (cond
     ((equal? 'find-indices (car line)) (fprintf out 
	 "(find-indices ~a ~a) = ~a~n" (car(cdr line)) (car(cddr line) )
	 (find-indices (car (cdr line)) (car(cddr line)))  ) )
     ((equal? 'remove (car line)) (fprintf out 
         "(remove ~a ~a) is ~a~n" (car (cdr line)) (car(cddr line) )
	 (remove (car (cdr line)) (car(cddr line)) )  ) )
     ((equal? 'asum (car line)) (fprintf out "(asum ~a) is ~a~n" 
	 (car (cdr line)) (asum (car(cdr line)) )  ) )
     ((equal? 'square (car line)) (fprintf out "(square ~a) is ~a~n" 
   	 (car (cdr line)) (square (car(cdr line)) )  ) )
     (else (displayln "I found something else"))
  )

  )

(close-output-port out )
