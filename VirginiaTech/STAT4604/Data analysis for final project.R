#importing ggplot and creating dataframes. the second dataframe is the reduced 
#set with the 321 entries that got more than 7 hours of screen time
df <- read.csv("ScreenTime vs MentalWellness.csv")
df2 <- read.csv("ScreenTime vs MentalWellness Subset.csv")
library(ggplot2)

#center calculations for screen time hours
screenMean = mean(df$screen_time_hours, na.rm = TRUE)
screenFiveNum = fivenum(df$screen_time_hours, na.rm = TRUE)
screenVar = var(df$screen_time_hours, na.rm = TRUE)
screenIqr = IQR(df$screen_time_hours, na.rm = TRUE)
screenMean
screenVar
screenFiveNum 
screenIqr

#center calculationgs for sleep hours
sleepMean = mean(df$sleep_hours, na.rm = TRUE)
sleepVar = var(df$sleep_hours, na.rm = TRUE)
sleepFiveNum = fivenum(df$sleep_hours, na.rm = TRUE)
sleepIqr = IQR(df$sleep_hours, na.rm = TRUE)
sleepMean
sleepVar
sleepFiveNum
sleepIqr

#plot a histogram for screen time hours
ggplot(df, aes(screen_time_hours)) +
  geom_histogram(
    bins = 15,
    color = "black",
    fill = "#183c40",
    alpha = 0.9
  ) +
  labs(
    title = "Screen time in hours",
    x = "Hours",
    y = "Count"
  ) +
  theme_minimal()

#plot a histogram for sleep hours
ggplot(df, aes(sleep_hours)) +
  geom_histogram(
    bins = 20,
    color = "black",
    fill = "#183c40",
    alpha = .9
  ) +
  labs(
    title = "Average Hours of Sleep",
    x = "Hours",
    y = "Count"
  ) +
  theme_minimal()


#greate a combined dataframe to plot both into one boxplot
combinedDF = data.frame(
  group = rep(c("Screen Time", "Sleep Hours"),
              times = c(length(df$screen_time_hours), length(df$sleep_hours))),
  value = c(df$screen_time_hours, df$sleep_hours)
)
#plol a combined boxplot
ggplot(combinedDF, aes(x= group, y = value))+
  geom_boxplot(
    width = 0.5,
    color = "black",
    fill = "#183c40",
    alpha = .9,
    outlier.shape = NA
  ) +
  labs(
    title = "5 number summary of Screen time and Sleep time in total hours",
    x = "Category",
    y = "Hours"
  ) +
  coord_flip() +
  theme_minimal()


#calculate binomial distribution and create a dataframe for variable k
k <- 0:321
pmf <- dbinom(k, size = 321, prob = 0.5545)
binomDF <- data.frame(k = k, pmf = pmf)

#plot the pmf of binomial with variable k for P(x = k)
ggplot(binomDF, aes(x = k, y = pmf)) +
  geom_col(
    color = "black",
    fill = "#183c40",
    alpha = 0.9,
    width = 1
  ) +
  coord_cartesian(xlim = c(100, 250)) +
  labs(
    title = "PMF of Binomial Distribution",
    x = "target number of successes (k)",
    y = "PMF(X = k)"
  ) +
  theme_minimal()


#calculate geometric distribution and create a dataframe for variable k
p <- 0.5545
q <- 1 - p
k <- 1:50   
pmf <- dgeom(k - 1, prob = p)   
geomDF <- data.frame(k = k, pmf = pmf)

#plot the pmf of geometric with variable k for P(x = k)
ggplot(geomDF, aes(x = k, y = pmf)) +
  geom_col(
    color = "black",
    fill = "#183c40",
    alpha = 0.9,
    width = .9
  ) +
  coord_cartesian(xlim = c(0, 15)) +
  scale_x_continuous(breaks = k) +
  labs(
    title = "PMF of Geometric Distribution",
    x = "Number of trials until first success (k)",
    y = "P(X = k)"
  ) +
  theme_minimal()
  
#create a linear regression model, store the coefficients, and calculate r^2
model <- lm(sleep_hours ~ screen_time_hours, data = df)
a <- coef(model)[2]
b <- coef(model)[1]
r2 <- summary(model)$r.squared
formula_text <- paste0("y = ", round(a, 3), "x + ", round(b, 3))
formula_text
#plot the scatterplot with linear regression line
ggplot(df, aes(x = screen_time_hours, y = sleep_hours)) +
  geom_point(color = "#183c40", size = 3, alpha = 0.7) +
  geom_smooth(method = "lm", se = FALSE, color = "red") +
  annotate("text", x = Inf, y = Inf,
           label = paste0(" y[i] == ", (round(a,3))," * x[i] + ",(round(b,3))),
           hjust = 1.1, vjust = 1.5, size = 5,
           parse = TRUE) +
  labs(
    title = "Screen time versus Sleep hours per participant",
    x = "Screen Time in Hours",
    y = "Sleep Time in Hours"
  ) +
  theme_minimal()
r2
  






