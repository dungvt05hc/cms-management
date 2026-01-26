'use client';

import React, { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import { ChevronLeftIcon, ChevronRightIcon } from '@/components/ui/Icons';
import styles from './HeroSlider.module.css';

interface Slide {
  id: string;
  title: string;
  subtitle: string;
  description: string;
  buttonText: string;
  buttonLink: string;
  image: string;
  bgColor: string;
}

const defaultSlides: Slide[] = [
  {
    id: '1',
    title: 'New Collection 2026',
    subtitle: 'Spring Sale',
    description: 'Discover amazing deals up to 50% off on our latest collection. Free shipping on orders over $50.',
    buttonText: 'Shop Now',
    buttonLink: '/products',
    image: '',
    bgColor: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
  },
  {
    id: '2',
    title: 'Premium Quality',
    subtitle: 'Best Sellers',
    description: 'Explore our most popular products loved by thousands of customers worldwide.',
    buttonText: 'Explore',
    buttonLink: '/products?featured=true',
    image: '',
    bgColor: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
  },
  {
    id: '3',
    title: 'Exclusive Deals',
    subtitle: 'Limited Time',
    description: 'Don\'t miss out on exclusive offers. Get up to 70% off on selected items.',
    buttonText: 'View Deals',
    buttonLink: '/deals',
    image: '',
    bgColor: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
  },
];

interface HeroSliderProps {
  slides?: Slide[];
  autoPlayInterval?: number;
}

export default function HeroSlider({ slides = defaultSlides, autoPlayInterval = 5000 }: HeroSliderProps) {
  const [currentSlide, setCurrentSlide] = useState(0);
  const [isAutoPlaying, setIsAutoPlaying] = useState(true);

  const nextSlide = useCallback(() => {
    setCurrentSlide((prev) => (prev + 1) % slides.length);
  }, [slides.length]);

  const prevSlide = useCallback(() => {
    setCurrentSlide((prev) => (prev - 1 + slides.length) % slides.length);
  }, [slides.length]);

  const goToSlide = (index: number) => {
    setCurrentSlide(index);
    setIsAutoPlaying(false);
    setTimeout(() => setIsAutoPlaying(true), 10000);
  };

  useEffect(() => {
    if (!isAutoPlaying) return;
    
    const interval = setInterval(nextSlide, autoPlayInterval);
    return () => clearInterval(interval);
  }, [isAutoPlaying, nextSlide, autoPlayInterval]);

  return (
    <section className={styles.heroSlider}>
      <div className={styles.slidesContainer}>
        {slides.map((slide, index) => (
          <div
            key={slide.id}
            className={`${styles.slide} ${index === currentSlide ? styles.active : ''}`}
            style={{ background: slide.bgColor }}
          >
            <div className={styles.slideContent}>
              <div className={styles.slideText}>
                <span className={styles.subtitle}>{slide.subtitle}</span>
                <h1 className={styles.title}>{slide.title}</h1>
                <p className={styles.description}>{slide.description}</p>
                <Link href={slide.buttonLink} className={styles.ctaButton}>
                  {slide.buttonText}
                </Link>
              </div>
              <div className={styles.slideImage}>
                {slide.image ? (
                  <img src={slide.image} alt={slide.title} />
                ) : (
                  <div className={styles.imagePlaceholder}>
                    <span>🛍️</span>
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Navigation Arrows */}
      <button 
        className={`${styles.navButton} ${styles.prevButton}`}
        onClick={prevSlide}
        aria-label="Previous slide"
      >
        <ChevronLeftIcon size={24} />
      </button>
      <button 
        className={`${styles.navButton} ${styles.nextButton}`}
        onClick={nextSlide}
        aria-label="Next slide"
      >
        <ChevronRightIcon size={24} />
      </button>

      {/* Dots Indicator */}
      <div className={styles.dotsContainer}>
        {slides.map((_, index) => (
          <button
            key={index}
            className={`${styles.dot} ${index === currentSlide ? styles.activeDot : ''}`}
            onClick={() => goToSlide(index)}
            aria-label={`Go to slide ${index + 1}`}
          />
        ))}
      </div>
    </section>
  );
}
